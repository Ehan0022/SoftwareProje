using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopSphere.Domain.Entities;

namespace ShopSphere.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Description).IsRequired().HasMaxLength(2000);
        builder.Property(p => p.Price).IsRequired().HasPrecision(18, 2);
        builder.Property(p => p.StockQuantity).IsRequired();
        builder.Property(p => p.IsActive).HasDefaultValue(true);
        builder.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

        // Check constraints
        builder.ToTable(t => t.HasCheckConstraint("CK_Product_Price", "[Price] >= 0"));
        builder.ToTable(t => t.HasCheckConstraint("CK_Product_StockQuantity", "[StockQuantity] >= 0"));

        builder.HasOne(p => p.Seller)
               .WithMany(s => s.Products)
               .HasForeignKey(p => p.SellerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Category)
               .WithMany(c => c.Products)
               .HasForeignKey(p => p.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
