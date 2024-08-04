using Dev2C2P.Services.Platform.Domain;

namespace Dev2C2P.Services.Platform.Application.Abstractions;

public interface ITransactionService
{
    Task<IEnumerable<Transaction>> GetAsync(
        Expression<Func<Transaction, bool>>? filter = null,
        Func<IQueryable<Transaction>, IOrderedQueryable<Transaction>>? orderBy = null,
        int offset = 0,
        int page = 1,
        int limit = 10
    );

    Task<Transaction?> GetByIdAsync(string id);

    Task<bool> UpdateOne(Transaction entity);
}
