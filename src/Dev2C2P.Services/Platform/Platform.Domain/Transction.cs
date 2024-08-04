using System.ComponentModel.DataAnnotations;
using Dev2C2P.Services.Platform.Domain.Abstractions;

namespace Dev2C2P.Services.Platform.Domain;

public class Transaction : Entity<long, string>, IIdentifiable<long>
{
    public string TransactionId { get; private set; }

    public DateTime At { get; private set; }

    public string Currency { get; private set; }

    public string Status { get; private set; }

    public decimal Amount { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    private Transaction(
        string transactionId,
        decimal amount,
        string currency,
        string status,
        DateTime at,
        DateTime createdAt,
        DateTime? updatedAt
    ) : base()
    {
        TransactionId = transactionId;
        Amount = amount;
        Currency = currency;
        Status = status;
        At = at;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    private Transaction(
        long id,
        string transactionId,
        decimal amount,
        string currency,
        string status,
        DateTime at,
        DateTime createdAt,
        DateTime? updatedAt
    ) : base(id)
    {
        TransactionId = transactionId;
        Amount = amount;
        Currency = currency;
        Status = status;
        At = at;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public override string GetId()
    {
        return TransactionId;
    }

    public override void SetId(string uniqueId)
    {
        TransactionId = uniqueId;
    }

    public static Transaction Create(
        string transactionId,
        decimal amount,
        string currency,
        string status,
        DateTime at
    )
    {
        return new Transaction(
            transactionId,
            amount,
            currency,
            status,
            at,
            DateTime.Now,
            null
        );
    }

    public static Transaction From(
        long id,
        string transactionId,
        decimal amount,
        string currency,
        string status,
        DateTime at,
        DateTime createdAt
    )
    {
        return new Transaction(
            id,
            transactionId,
            amount,
            currency,
            status,
            at,
            createdAt,
            DateTime.Now
        );
    }
}
