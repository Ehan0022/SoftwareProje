using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopSphere.Domain.Entities;

namespace ShopSphere.Infrastructure.Persistence.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.City).IsRequired().HasMaxLength(100);
        builder.Property(a => a.District).IsRequired().HasMaxLength(100);
        builder.Property(a => a.FullAddress).IsRequired().HasMaxLength(500);

        builder.HasOne(a => a.Customer)
               .WithMany(cp => cp.Addresses)
               .HasForeignKey(a => a.CustomerId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
