namespace ShopSphere.Domain.Entities;

public class Address
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string City { get; set; } = null!;
    public string District { get; set; } = null!;
    public string FullAddress { get; set; } = null!;

    // Navigation
    public CustomerProfile Customer { get; set; } = null!;
}
