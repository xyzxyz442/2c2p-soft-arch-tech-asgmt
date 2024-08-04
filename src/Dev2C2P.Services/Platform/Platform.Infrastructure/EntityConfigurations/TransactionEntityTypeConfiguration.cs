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
        builder.HasIndex(e => e.TransactionId)
            .IsUnique();

        builder.Property(e => e.TransactionId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Amount)
            .IsRequired()
            .HasColumnType("decimal(18, 2)");

        builder.Property(e => e.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(1);

        builder.Property(e => e.At)
            .IsRequired()
            .HasColumnType("timestamp without time zone");

        // audit columns
        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp without time zone");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone");

    }
}
