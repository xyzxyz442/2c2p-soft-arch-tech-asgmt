using Dev2C2P.Services.Platform.Application.Abstractions;
using Dev2C2P.Services.Platform.Contracts.Transactions.Dtos;

namespace Dev2C2P.Services.Platform.Application.Transactions.Queries;

public record GetTransactionsQuery
    : RequestBase<IEnumerable<TransactionDto>>
{
    public string? Currency { get; init; }

    public string? From { get; init; }

    public string? To { get; init; }

    public string? Status { get; init; }

    public GetTransactionsQuery(
        string? currency,
        string? from,
        string? to,
        string? status)
    {
        Currency = currency;
        From = from;
        To = to;
        Status = status;
    }
}
