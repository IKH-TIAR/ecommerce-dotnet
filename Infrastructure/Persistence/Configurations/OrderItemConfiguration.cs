using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Quantity)
            .IsRequired();

        builder.Property(i => i.Size)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(i => i.UnitPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        // 1 Order -> Many Items
        builder.HasOne(i => i.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // 1 Jersey -> Many OrderItems (Cannot delete jersey if it exists in past order history!)
        builder.HasOne(i => i.Jersey)
            .WithMany()
            .HasForeignKey(i => i.JerseyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
