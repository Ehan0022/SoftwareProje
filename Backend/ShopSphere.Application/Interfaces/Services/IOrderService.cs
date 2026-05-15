using ShopSphere.Application.DTOs.Common;
using ShopSphere.Application.DTOs.Orders;

namespace ShopSphere.Application.Interfaces.Services;

public interface IOrderService
{
    Task<ApiResponseDto> CheckoutAsync(int customerId, CheckoutRequestDto request);
    Task<ApiResponseDto> GetMyOrdersAsync(int customerId);
    Task<ApiResponseDto> GetOrderByIdAsync(int userId, int orderId, bool isAdmin = false);
}
