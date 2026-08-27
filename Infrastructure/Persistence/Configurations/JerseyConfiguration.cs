using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecommerce.Infrastructure.Persistence.Configurations;

public class JerseyConfiguration : IEntityTypeConfiguration<Jersey>
{
    public void Configure(EntityTypeBuilder<Jersey> builder)
    {
        builder.ToTable("Jerseys");

        builder.HasKey(j => j.Id);

        builder.Property(j => j.Name).IsRequired().HasMaxLength(150);

        builder.Property(j => j.Price).HasPrecision(18, 2);

        builder.HasOne(j => j.Club)
        .WithMany(c => c.Jerseys)
        .HasForeignKey(j => j.ClubId)
        .OnDelete(DeleteBehavior.Restrict);
    }
}