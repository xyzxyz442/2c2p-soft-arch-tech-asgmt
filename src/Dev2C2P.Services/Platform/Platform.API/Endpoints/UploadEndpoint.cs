using System.Xml;
using CsvHelper;
using CsvHelper.Configuration;
using Dev2C2P.Services.Platform.Contracts.Xml;
using ErrorOr;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Dev2C2P.Services.Platform.API.Endpoints;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class DisableFormValueModelBindingAttribute : Attribute, IResourceFilter
{
    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        var factories = context.ValueProviderFactories;
        factories.RemoveType<FormValueProviderFactory>();
        factories.RemoveType<FormFileValueProviderFactory>();
        factories.RemoveType<JQueryFormValueProviderFactory>();
    }
    public void OnResourceExecuted(ResourceExecutedContext context)
    {
    }
}

[Route("upload")]
[DisableFormValueModelBinding]
[ApiExplorerSettings(GroupName = "upload")]
public class UploadEndpoint : EndpointBaseAsync.WithoutRequest.WithResult<IActionResult>
{
    private readonly ILogger<UploadEndpoint> logger;
    private readonly IOptionsMonitor<ApplicationSettings> options;

    public UploadEndpoint(
        ILogger<UploadEndpoint> logger,
        IOptionsMonitor<ApplicationSettings> options
    )
    {
        this.logger = logger;
        this.options = options;
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Upload file",
        Description = "Upload file",
        OperationId = "Upload.Upload",
        Tags = new[] { "Upload" }
    )]
    public override async Task<IActionResult> HandleAsync(CancellationToken cancellationToken = default)
    {
        var request = HttpContext.Request;

        if (!request.HasFormContentType ||
                !MediaTypeHeaderValue.TryParse(request.ContentType, out var mediaTypeHeader) ||
                string.IsNullOrEmpty(mediaTypeHeader.Boundary.Value))
        {
            return BadRequest(new
            {
                errors = new[] {
                    new { code = "UploadValidation", message = "Invalid content type, multipart/form-data must contain boundary." }
                }
            });
        }

        var result = await DoHandleAsync(request, mediaTypeHeader);

        return result.Match<IActionResult>(
            success => Ok(new { data = success }),
            errors => BadRequest(new { errors = errors.Select(e => new { code = e.Code, message = e.Description }) })
        );
    }

    private async Task<ErrorOr<bool>> DoHandleAsync(HttpRequest request, MediaTypeHeaderValue mediaTypeHeader)
    {
        try
        {
            var boundary = HeaderUtilities.RemoveQuotes(mediaTypeHeader.Boundary).Value;
            var reader = new MultipartReader(boundary, request.Body);
            var section = await reader.ReadNextSectionAsync();

            var dirPath = "tmp";

            var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var tmpFilePath = Path.Combine(dirPath, Path.GetRandomFileName() + "_" + timestamp);

            var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(
                section.ContentDisposition,
                out var contentDisposition);

            if (hasContentDispositionHeader && contentDisposition.DispositionType.Equals("form-data") &&
                    !string.IsNullOrEmpty(contentDisposition.FileName.Value))
            {
                var validationResult = ValidateMultiPartSection(section, contentDisposition);
                if (validationResult.IsError) return validationResult;

                using (var stream = System.IO.File.Create(tmpFilePath))
                {
                    await section.Body.CopyToAsync(stream);
                }

                var importResult = await ImportFileAsync(contentDisposition.FileName.Value, tmpFilePath);

                if (System.IO.File.Exists(tmpFilePath)) System.IO.File.Delete(tmpFilePath);

                if (importResult.IsError) return importResult;
            }

            return true;
        }
        catch (BadHttpRequestException ex)
        {
            logger.LogError(ex, "UploadError");
            return Error.Failure(ex.Message);
        }
    }

    private ErrorOr<bool> ValidateMultiPartSection(
        MultipartSection section,
        ContentDispositionHeaderValue contentDispositionHeaderValue)
    {
        // TODO: should change this to configuration
        var maxFileSize = 1 * 1024 * 1024; // 1MB

        var sectionLength = section.Body.Length;

        // check file is size must not exceed 1MB
        if (sectionLength > maxFileSize)
        {
            return Error.Validation("UploadValidation", "The file is too large.");
        }

        // check file type
        var fileName = contentDispositionHeaderValue.FileName.Value;
        var fileExtension = Path.GetExtension(fileName);

        if (fileExtension != ".xml" && fileExtension != ".csv")
        {
            return Error.Validation("UploadValidation", "Invalid file type. Only XML and CSV files are allowed.");
        }

        return true;
    }

    private async Task<ErrorOr<bool>> ImportFileAsync(string originalFileName, string filePath)
    {
        var fileExtension = Path.GetExtension(originalFileName);
        if (fileExtension == ".xml")
        {
            var result = ParseXmlFile(filePath);
            if (result.IsError) return result.FirstError;

            var dtos = result.Value;

            // TODO: send command import with XML data
            return false;
        }
        else if (fileExtension == ".csv")
        {
            var result = await ParseCsvFileAsync(filePath);
            if (result.IsError) return result.FirstError;

            var dtos = result.Value;

            // TODO: send command import with CSV data
            return false;
        }
        else
        {
            return Error.Failure("UploadParseError", "Invalid file type.");
        }
    }

    private ErrorOr<IEnumerable<TransactionXmlDto>> ParseXmlFile(string filePath)
    {
        try
        {
            var dtos = new List<TransactionXmlDto>();

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);

            if (xmlDoc.DocumentElement == null)
            {
                return Error.Failure("UploadXmlParseError", "Invalid XML file.");
            }

            var serializer = new XmlSerializer(typeof(TransactionsXmlDto));
            var dto = serializer.Deserialize(new XmlNodeReader(xmlDoc.DocumentElement)) as TransactionsXmlDto;
            if (dto == null)
            {
                return Error.Failure("UploadXmlParseError", "Invalid XML file.");
            }

            dtos.AddRange(dto.Transactions);

            return dtos;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "UploadXmlParseError");
            return Error.Failure("UploadXmlParseError", ex.Message);
        }
    }

    private async Task<ErrorOr<IEnumerable<TransactionCsvDto>>> ParseCsvFileAsync(string filePath)
    {
        try
        {
            var dtos = new List<TransactionCsvDto>();

            using (var sr = System.IO.File.OpenText(filePath))
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = false,
                    MissingFieldFound = null,
                    TrimOptions = TrimOptions.Trim,
                };

                using var csvReader = new CsvReader(sr, config);

                while (await csvReader.ReadAsync())
                {
                    // TODO: should parse and clean to DTO to send to command
                    var dto = new TransactionCsvDto
                    {
                        TransactionId = csvReader.GetField(0),
                        Amount = csvReader.GetField<decimal>(1),
                        CurrencyCode = csvReader.GetField(2),
                        TransactionDate = csvReader.GetField(3),
                        Status = csvReader.GetField(4)
                    };
                }
            }

            return dtos;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "UploadCsvParseError");
            return Error.Failure("UploadCsvParseError", ex.Message);
        }
    }
}
