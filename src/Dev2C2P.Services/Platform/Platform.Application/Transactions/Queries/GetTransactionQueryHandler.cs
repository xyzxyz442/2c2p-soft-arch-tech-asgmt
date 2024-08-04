using Dev2C2P.Services.Platform.Application.Abstractions;
using Dev2C2P.Services.Platform.Common;
using Dev2C2P.Services.Platform.Contracts.Transactions.Dtos;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Dev2C2P.Services.Platform.Application.Transactions.Queries;

public class GetTransactionsQueryHandler
    : RequestHandlerBase<GetTransactionsQuery, IEnumerable<TransactionDto>>
{
    public GetTransactionsQueryHandler(
        ILogger<RequestHandlerBase<GetTransactionsQuery,
        IEnumerable<TransactionDto>>> logger)
        : base(logger)
    {
    }

    protected override async Task<ErrorOr<Result<IEnumerable<TransactionDto>>>> DoHandle(GetTransactionsQuery request, CancellationToken cancellationToken)
    {
        var dtos = new List<TransactionDto>();

        // TODO: actual query to data service or repository here.

        return new Result<IEnumerable<TransactionDto>>(dtos);
    }

    protected override string GetLogPrefix()
    {
        return nameof(GetTransactionsQueryHandler);
    }
}
