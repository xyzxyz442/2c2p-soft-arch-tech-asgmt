namespace Dev2C2P.Services.Platform.Contracts.Transactions;

public record ImportTransactionInput
{
    public string Id { get; init; } = string.Empty;

    public string At { get; init; } = string.Empty;

    public decimal Amount { get; init; } = 0;

    public string CurrencyCode { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

}
