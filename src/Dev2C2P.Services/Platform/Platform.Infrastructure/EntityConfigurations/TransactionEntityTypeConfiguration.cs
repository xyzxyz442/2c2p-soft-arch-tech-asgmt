using Dev2C2P.Services.Platform.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dev2C2P.Services.Platform.Infrastructure.EntityConfigurations;

public class TransactionEntityTypeConfiguration : IEntityTypeConfiguration<Transaction>
{
    private string _tableName;

    public TransactionEntityTypeConfiguration(string tableName)
    {
        _tableName = tableName;
    }

    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable(_tableName);

        builder.HasKey(e => new { e.Id });

        // TODO: add another column here.
    }
}
