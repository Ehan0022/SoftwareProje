namespace ShopSphere.Domain.Entities;

public class SellerProfile
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string StoreName { get; set; } = null!;

    // Navigation
    public User User { get; set; } = null!;
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
