using Dev2C2P.Services.Platform.Domain.Abstractions;

namespace Dev2C2P.Services.Platform.Infrastructure;

public abstract class EFDbContext<T, TId>([NotNull] DbContextOptions options) : DbContext(options)
    where T : class, IIdentifiable<TId>
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        DoModelCreating(modelBuilder);
    }

    protected abstract void DoModelCreating(ModelBuilder modelBuilder);
}
