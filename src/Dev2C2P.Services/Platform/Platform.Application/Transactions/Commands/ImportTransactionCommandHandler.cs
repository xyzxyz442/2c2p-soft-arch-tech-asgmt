using Dev2C2P.Services.Platform.Application.Abstractions;
using Dev2C2P.Services.Platform.Common;
using Dev2C2P.Services.Platform.Domain;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Dev2C2P.Services.Platform.Application.Transactions.Commands;

public class ImportTrasactionCommandHandler
    : RequestHandlerBase<ImportTransactionCommand, bool>
{

    public ImportTrasactionCommandHandler(
        ILogger<ImportTrasactionCommandHandler> logger
    ) : base(logger)
    {
    }

    protected override async Task<ErrorOr<Result<bool>>> DoHandle(
        ImportTransactionCommand request,
        CancellationToken cancellationToken)
    {

        foreach (var data in request.Datas)
        {
            Logger.LogDebug("Importing transaction data: {data}", data);

            if (request.Type == ImportTransactionFileType.Csv)
            {
                // var transaction = Transaction.Create(
                //     data.Id,
                //     data.At,
                //     data.Amount,
                //     data.CurrencyCode,
                //     data.Status
                // );

                // TODO: actual call to service/repository to create transaction

                // if (result.IsFailure)
                // {
                //     Logger.LogError("Failed to create transaction: {Id}, error: {Error}", data.Id, result.Error);
                //     return new Result<bool>(false);
                // }

            }
            else if (request.Type == ImportTransactionFileType.Xml)
            {
                // TODO: actual call to service/repository to create transaction
            }
            else
            {
                Logger.LogError("Invalid file type: {Type}, id: {TransactionId}", request.Type, data.Id);
                return new Result<bool>(false);
            }
        }

        return new Result<bool>(true);
    }

    protected override string GetLogPrefix()
    {
        return nameof(ImportTrasactionCommandHandler);
    }

    // private virtual async Task<TransactionDto> MapAsync<T>(Transaction transaction)
    // {
    //     // TODO: do transaction map to DTO here
    // }
}
