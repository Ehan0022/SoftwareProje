using ShopSphere.Application.DTOs.Common;
using ShopSphere.Application.DTOs.Orders;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Interfaces.Repositories;
using ShopSphere.Application.Interfaces.Services;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Enums;

namespace ShopSphere.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(IOrderRepository orderRepository, ICartRepository cartRepository, IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponseDto> CheckoutAsync(int customerId, CheckoutRequestDto request)
    {
        var cart = await _cartRepository.GetCartByCustomerIdAsync(customerId);
        if (cart == null || !cart.CartItems.Any()) return ApiResponseDto.FailureResponse("Cart is empty");

        // Validate stock
        foreach (var item in cart.CartItems)
        {
            if (item.Product.StockQuantity < item.Quantity)
            {
                return ApiResponseDto.FailureResponse($"Insufficient stock for product: {item.Product.Name}");
            }
        }

        // Create Order
        var order = new Order
        {
            CustomerId = customerId,
            Status = OrderStatus.Created,
            TotalAmount = cart.CartItems.Sum(i => i.Quantity * i.Product.Price),
            OrderItems = cart.CartItems.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.Product.Price
            }).ToList()
        };

        // Reduce stock
        foreach (var item in cart.CartItems)
        {
            item.Product.StockQuantity -= item.Quantity;
            _productRepository.Update(item.Product);
        }

        // Simulate payment
        order.Payment = new Payment
        {
            Amount = order.TotalAmount,
            PaymentStatus = "Success",
            CreatedAt = DateTime.UtcNow
        };
        order.Status = OrderStatus.Confirmed;

        await _orderRepository.AddAsync(order);

        // Clear cart
        foreach (var item in cart.CartItems.ToList())
        {
            cart.CartItems.Remove(item);
        }

        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto.SuccessResponse(order.Id, "Order placed successfully");
    }

    public async Task<ApiResponseDto> GetMyOrdersAsync(int customerId)
    {
        var orders = await _orderRepository.GetOrdersByCustomerIdAsync(customerId);
        var response = orders.Select(o => new OrderResponseDto
        {
            Id = o.Id,
            TotalAmount = o.TotalAmount,
            Status = o.Status,
            CreatedAt = o.CreatedAt
        }).ToList();

        return ApiResponseDto.SuccessResponse(response);
    }

    public async Task<ApiResponseDto> GetOrderByIdAsync(int userId, int orderId, bool isAdmin = false)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null) return ApiResponseDto.FailureResponse("Order not found");
        if (!isAdmin && order.CustomerId != userId) return ApiResponseDto.FailureResponse("Access denied");

        var response = new OrderResponseDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            CreatedAt = order.CreatedAt,
            Items = order.OrderItems.Select(oi => new OrderItemResponseDto
            {
                ProductId = oi.ProductId,
                ProductName = oi.Product.Name,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice
            }).ToList()
        };

        return ApiResponseDto.SuccessResponse(response);
    }
}
