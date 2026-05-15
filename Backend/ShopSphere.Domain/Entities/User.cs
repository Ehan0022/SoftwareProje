using ShopSphere.Domain.Enums;

namespace ShopSphere.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public CustomerProfile? CustomerProfile { get; set; }
    public SellerProfile? SellerProfile { get; set; }
    public AdminProfile? AdminProfile { get; set; }
}
