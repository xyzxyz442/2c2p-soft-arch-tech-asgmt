using System.Globalization;
using Dev2C2P.Services.Platform.Application.Abstractions;
using Dev2C2P.Services.Platform.Common;
using Dev2C2P.Services.Platform.Domain;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Dev2C2P.Services.Platform.Application.Transactions.Commands;

public class ImportTransactionCommandHandler
    : RequestHandlerBase<ImportTransactionCommand, bool>
{
    private readonly ITransactionService _service;

    public ImportTransactionCommandHandler(
        ITransactionService service,
        ILogger<ImportTransactionCommandHandler> logger
    ) : base(logger)
    {
        _service = service;
    }

    protected override async Task<ErrorOr<Result<bool>>> DoHandle(
        ImportTransactionCommand request,
        CancellationToken cancellationToken)
    {

        foreach (var data in request.Datas)
        {
            Logger.LogDebug("Importing transaction data: {data}", data);

            // TODO: this might concern race-condition, revise is advised.
            var existEntity = await _service.GetByIdAsync(data.Id);

            var status = "";

            switch (data.Status)
            {
                case "Approved":
                    status = "A";
                    break;
                case "Failed":
                case "Rejected":
                    status = "R";
                    break;
                case "Finished":
                case "Done":
                    status = "D";
                    break;

            }

            Transaction? entity;
            DateTime at = DateTime.MinValue;

            if (request.Type == ImportTransactionFileType.Csv)
            {
                at = DateTime.ParseExact(data.At, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            }
            else if (request.Type == ImportTransactionFileType.Xml)
            {
                at = DateTime.ParseExact(data.At, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
            }
            else
            {
                var error = string.Format($"Invalid file type: {request.Type}, id: {data.Id}");
                Logger.LogError(error);
                return Error.Failure("Exception", error);
            }

            if (existEntity is null)
            {
                entity = Transaction.Create(
                    data.Id,
                    data.Amount,
                    data.CurrencyCode,
                    status,
                    at
                );
            }
            else
            {
                entity = Transaction.From(
                    existEntity.Id,
                    data.Id,
                    data.Amount,
                    data.CurrencyCode,
                    status,
                    at,
                    DateTime.Now
                );
            }

            var result = await _service.UpdateOne(entity);
            if (!result)
            {
                Logger.LogError("Failed to import transaction data: {Data}", data);
                return Error.Failure("Exception", "Failed to import transaction data");
            }
        }

        Logger.LogDebug("Transaction datas imported: {request.Datas.Count()} rows.");

        return new Result<bool>(true);
    }

    protected override string GetLogPrefix()
    {
        return nameof(ImportTransactionCommandHandler);
    }

    // private virtual async Task<TransactionDto> MapAsync<T>(Transaction transaction)
    // {
    //     // TODO: do transaction map to DTO here
    // }
}
