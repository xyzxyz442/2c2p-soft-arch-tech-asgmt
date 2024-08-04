using Dev2C2P.Services.Platform.Domain;
using Dev2C2P.Services.Platform.Infrastructure.EntityConfigurations;

namespace Dev2C2P.Services.Platform.Infrastructure.Persistences;

public class TransactionDbContext : EFDbContext<Transaction, long>
{
    private string _tableName;

    public DbSet<Transaction> Transactions { get; set; }

    public TransactionDbContext(
        DbContextOptions<TransactionDbContext> options
    ) : base(options)
    {
        _tableName = "transaction";
    }

    protected override void DoModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TransactionEntityTypeConfiguration(_tableName));
    }
}
