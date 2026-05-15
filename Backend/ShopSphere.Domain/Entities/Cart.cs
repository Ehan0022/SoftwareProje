namespace ShopSphere.Domain.Entities;

public class Cart
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public CustomerProfile Customer { get; set; } = null!;
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
