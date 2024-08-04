namespace Dev2C2P.Services.Platform.Contracts.Transactions.Dtos.Csv;

public record TransactionCsvDto
{
    public string TransactionId { get; init; } = string.Empty;
    public string TransactionDate { get; init; } = string.Empty;
    public decimal Amount { get; init; } = 0;
    public string CurrencyCode { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
}
