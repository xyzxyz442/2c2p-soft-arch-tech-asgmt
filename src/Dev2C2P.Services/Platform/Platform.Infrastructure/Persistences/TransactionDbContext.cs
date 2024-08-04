using Dev2C2P.Services.Platform.Domain;
using Dev2C2P.Services.Platform.Infrastructure.EntityConfigurations;

namespace Dev2C2P.Services.Platform.Infrastructure.Persistences;

public class TransactionDbContext : EFDbContext<Transaction, long>
{
    private string _tableName;

    public DbSet<Transaction> Transactions { get; set; }

    public TransactionDbContext(
        string tableName,
        DbContextOptions<TransactionDbContext> options
    ) : base(options)
    {
        _tableName = tableName;
    }

    protected override void DoModelCreating(ModelBuilder modelBuilder)
    {
        // TODO: check database is postgresql
        // if (Database.IsNpgsql())
        // {
        //     var converter = new ValueConverter<byte[], long>(
        //         v => BitConverter.ToInt64(v, 0),
        //         v => BitConverter.GetBytes(v));

        //     modelBuilder.Entity<Transaction>()
        //         .Property(_ => _.Version)
        //         .HasColumnName("xmin")
        //         .HasColumnType("xid")
        //         .HasConversion(converter);
        // }

        modelBuilder.ApplyConfiguration(new TransactionEntityTypeConfiguration(_tableName));
    }
}
