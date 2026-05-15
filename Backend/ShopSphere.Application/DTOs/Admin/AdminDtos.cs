namespace ShopSphere.Application.DTOs.Admin;

public class AdminDashboardResponseDto
{
    public int TotalCustomers { get; set; }
    public int TotalSellers { get; set; }
    public int TotalProducts { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
}
