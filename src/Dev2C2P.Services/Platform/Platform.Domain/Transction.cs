using Dev2C2P.Services.Platform.Domain.Abstractions;

namespace Dev2C2P.Services.Platform.Domain;

public class Transaction : Entity<long, string>
{
    public string TransactionId { get; set; }

    public string Currency { get; set; }

    // public TransctionStatus Status { get; set; }

    public decimal Amount { get; set; }

    private Transaction(
        string transactionId
    )
    {
        TransactionId = transactionId;
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
        string transactionId
    )
    {
        return new Transaction(
            transactionId: transactionId
        );
    }

}
