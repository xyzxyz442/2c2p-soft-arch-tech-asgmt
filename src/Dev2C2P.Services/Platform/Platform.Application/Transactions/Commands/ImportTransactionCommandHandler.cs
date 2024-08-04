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
            // TODO: import transaction data here
            Logger.LogDebug("Importing transaction data: {data}", data);
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
