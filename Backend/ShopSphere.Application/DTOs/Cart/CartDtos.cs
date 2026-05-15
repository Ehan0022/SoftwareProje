namespace ShopSphere.Application.DTOs.Cart;

public class AddCartItemRequestDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class UpdateCartItemRequestDto
{
    public int Quantity { get; set; }
}

public class CartItemResponseDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice => UnitPrice * Quantity;
}

public class CartResponseDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public List<CartItemResponseDto> Items { get; set; } = new();
    public decimal TotalAmount => Items.Sum(x => x.TotalPrice);
}
