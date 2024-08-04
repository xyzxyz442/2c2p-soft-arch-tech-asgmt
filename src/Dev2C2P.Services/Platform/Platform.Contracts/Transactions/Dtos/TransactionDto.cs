namespace Dev2C2P.Services.Platform.Contracts.Transactions.Dtos;

public record TransactionDto(
    string Id,
    decimal Amount,
    string Currency,
    DateTime At,
    string Status
);
