using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopSphere.Domain.Entities;

namespace ShopSphere.Infrastructure.Persistence.Configurations;

public class SellerProfileConfiguration : IEntityTypeConfiguration<SellerProfile>
{
    public void Configure(EntityTypeBuilder<SellerProfile> builder)
    {
        builder.HasKey(sp => sp.Id);
        builder.HasIndex(sp => sp.UserId).IsUnique();
        builder.Property(sp => sp.StoreName).IsRequired().HasMaxLength(100);

        builder.HasOne(sp => sp.User)
               .WithOne(u => u.SellerProfile)
               .HasForeignKey<SellerProfile>(sp => sp.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
