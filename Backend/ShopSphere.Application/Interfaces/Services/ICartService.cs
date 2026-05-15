using ShopSphere.Application.DTOs.Cart;
using ShopSphere.Application.DTOs.Common;

namespace ShopSphere.Application.Interfaces.Services;

public interface ICartService
{
    Task<ApiResponseDto> GetCartAsync(int customerId);
    Task<ApiResponseDto> AddItemToCartAsync(int customerId, AddCartItemRequestDto request);
    Task<ApiResponseDto> UpdateCartItemAsync(int customerId, int cartItemId, UpdateCartItemRequestDto request);
    Task<ApiResponseDto> RemoveCartItemAsync(int customerId, int cartItemId);
}
