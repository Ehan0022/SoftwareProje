using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopSphere.Domain.Entities;

namespace ShopSphere.Infrastructure.Persistence.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(ci => ci.Id);
        builder.Property(ci => ci.Quantity).IsRequired();

        // Check constraint
        builder.ToTable(t => t.HasCheckConstraint("CK_CartItem_Quantity", "[Quantity] > 0"));

        builder.HasOne(ci => ci.Cart)
               .WithMany(c => c.CartItems)
               .HasForeignKey(ci => ci.CartId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ci => ci.Product)
               .WithMany(p => p.CartItems)
               .HasForeignKey(ci => ci.ProductId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
