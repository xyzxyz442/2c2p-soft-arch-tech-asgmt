using Dev2C2P.Services.Platform.Application.Abstractions;
using Dev2C2P.Services.Platform.Domain;

namespace Dev2C2P.Services.Platform.Infrastructure.Persistences;

public class TransactionRepository
    : EFRepository<TransactionDbContext, Transaction, long, string>,
    ITransactionRepository
{
    public TransactionRepository(
        TransactionDbContext dbContext,
        ILogger<TransactionRepository> logger)
        : base(dbContext, logger)
    {
    }

    protected override Expression<Func<Transaction, bool>> BuildFilterByUniqueId<Transaction>(string uniqueId)
    {
        return (e) => e.TransactionId == uniqueId;
    }
}
