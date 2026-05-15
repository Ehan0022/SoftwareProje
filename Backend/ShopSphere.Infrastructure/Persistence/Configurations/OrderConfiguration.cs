using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopSphere.Domain.Entities;

namespace ShopSphere.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.TotalAmount).IsRequired().HasPrecision(18, 2);
        builder.Property(o => o.Status).IsRequired().HasConversion<string>();
        builder.Property(o => o.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(o => o.Customer)
               .WithMany(cp => cp.Orders)
               .HasForeignKey(o => o.CustomerId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
