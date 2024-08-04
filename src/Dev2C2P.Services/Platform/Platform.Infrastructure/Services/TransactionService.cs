using Dev2C2P.Services.Platform.Application.Abstractions;
using Dev2C2P.Services.Platform.Domain;

namespace Dev2C2P.Services.Platform.Infrastructure.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _repository;

    public TransactionService(ITransactionRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<Transaction>> GetAsync(
        Expression<Func<Transaction, bool>>? filter = null,
        Func<IQueryable<Transaction>, IOrderedQueryable<Transaction>>? orderBy = null,
        int offset = 0,
        int page = 1,
        int limit = 10
    )
    {
        int skip = ((page - 1) * limit) + offset;

        return _repository.GetAsync(
            filter,
            orderBy,
            skip,
            limit
        );
    }

    public Task<Transaction?> GetByIdAsync(string id)
    {
        return _repository.GetByUniqueIdAsync<Transaction>(id);
    }

    public Task<bool> UpdateOne(Transaction entity)
    {
        return _repository.UpdateOneAsync(entity);
    }
}
