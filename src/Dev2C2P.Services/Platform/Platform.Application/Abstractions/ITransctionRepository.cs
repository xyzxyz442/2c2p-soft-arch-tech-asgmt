using Dev2C2P.Services.Platform.Domain;

namespace Dev2C2P.Services.Platform.Application.Abstractions;

public interface ITransactionRepository : IEFRepository<Transaction, long, string>
{
}
