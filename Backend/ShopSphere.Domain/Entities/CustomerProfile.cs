namespace ShopSphere.Domain.Entities;

public class CustomerProfile
{
    public int Id { get; set; }
    public int UserId { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public Cart? Cart { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
