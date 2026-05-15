using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopSphere.Domain.Entities;

namespace ShopSphere.Infrastructure.Persistence.Configurations;

public class CustomerProfileConfiguration : IEntityTypeConfiguration<CustomerProfile>
{
    public void Configure(EntityTypeBuilder<CustomerProfile> builder)
    {
        builder.HasKey(cp => cp.Id);
        builder.HasIndex(cp => cp.UserId).IsUnique();
        
        builder.HasOne(cp => cp.User)
               .WithOne(u => u.CustomerProfile)
               .HasForeignKey<CustomerProfile>(cp => cp.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
