using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Persistence.Configurations;

public class JerseySizeStockConfiguration : IEntityTypeConfiguration<JerseySizeStock>
{
    public void Configure(EntityTypeBuilder<JerseySizeStock> builder)
    {
        builder.ToTable("JerseySizeStocks");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Size)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(s => s.StockQuantity)
            .IsRequired();

        // Enforce unique Size per Jersey: One Jersey cannot have two size 'M' rows!
        builder.HasIndex(s => new { s.JerseyId, s.Size })
            .IsUnique();

        builder.HasOne(s => s.Jersey)
            .WithMany(j => j.Sizes)
            .HasForeignKey(s => s.JerseyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
