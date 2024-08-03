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
    private readonly IOptionsMonitor<ApplicationSettings> options;

    public UploadEndpoint(
        IOptionsMonitor<ApplicationSettings> options
    )
    {
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
                var validationResult = await ValidateMultiPartSection(section, contentDisposition);
                if (validationResult.IsError) return validationResult;

                using (var stream = System.IO.File.Create(tmpFilePath))
                {
                    await section.Body.CopyToAsync(stream);
                }

                // TODO: reading file and parse data here
            }

            return true;
        }
        catch (BadHttpRequestException ex)
        {
            return Error.Failure(ex.Message);
        }
    }

    private async Task<ErrorOr<bool>> ValidateMultiPartSection(
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
}
