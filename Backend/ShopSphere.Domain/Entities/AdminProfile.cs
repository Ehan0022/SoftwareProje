namespace ShopSphere.Domain.Entities;

public class AdminProfile
{
    public int Id { get; set; }
    public int UserId { get; set; }

    // Navigation
    public User User { get; set; } = null!;
}
