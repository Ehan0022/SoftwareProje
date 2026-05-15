using Microsoft.EntityFrameworkCore;
using ShopSphere.Domain.Entities;

namespace ShopSphere.Infrastructure.Persistence;

public class ShopSphereDbContext : DbContext
{
    public ShopSphereDbContext(DbContextOptions<ShopSphereDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<CustomerProfile> CustomerProfiles => Set<CustomerProfile>();
    public DbSet<SellerProfile> SellerProfiles => Set<SellerProfile>();
    public DbSet<AdminProfile> AdminProfiles => Set<AdminProfile>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ShopSphereDbContext).Assembly);
    }
}
