using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopSphere.Domain.Entities;

namespace ShopSphere.Infrastructure.Persistence.Configurations;

public class AdminProfileConfiguration : IEntityTypeConfiguration<AdminProfile>
{
    public void Configure(EntityTypeBuilder<AdminProfile> builder)
    {
        builder.HasKey(ap => ap.Id);
        builder.HasIndex(ap => ap.UserId).IsUnique();

        builder.HasOne(ap => ap.User)
               .WithOne(u => u.AdminProfile)
               .HasForeignKey<AdminProfile>(ap => ap.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
