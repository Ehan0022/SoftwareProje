using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopSphere.Domain.Entities;

namespace ShopSphere.Infrastructure.Persistence.Configurations;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.HasKey(c => c.Id);
        builder.HasIndex(c => c.CustomerId).IsUnique();
        builder.Property(c => c.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(c => c.Customer)
               .WithOne(cp => cp.Cart)
               .HasForeignKey<Cart>(c => c.CustomerId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
