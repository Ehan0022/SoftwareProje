using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopSphere.Domain.Entities;

namespace ShopSphere.Infrastructure.Persistence.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Rating).IsRequired();
        builder.Property(r => r.Comment).IsRequired().HasMaxLength(1000);
        builder.Property(r => r.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

        // Check constraint
        builder.ToTable(t => t.HasCheckConstraint("CK_Review_Rating", "[Rating] BETWEEN 1 AND 5"));

        builder.HasOne(r => r.Product)
               .WithMany(p => p.Reviews)
               .HasForeignKey(r => r.ProductId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Customer)
               .WithMany(cp => cp.Reviews)
               .HasForeignKey(r => r.CustomerId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
