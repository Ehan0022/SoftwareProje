using ShopSphere.Application.DTOs.Cart;
using ShopSphere.Application.DTOs.Common;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Interfaces.Repositories;
using ShopSphere.Application.Interfaces.Services;
using ShopSphere.Domain.Entities;

namespace ShopSphere.Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly ICartItemRepository _cartItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CartService(ICartRepository cartRepository, ICartItemRepository cartItemRepository, IUnitOfWork unitOfWork)
    {
        _cartRepository = cartRepository;
        _cartItemRepository = cartItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponseDto> GetCartAsync(int customerId)
    {
        var cart = await _cartRepository.GetCartByCustomerIdAsync(customerId);
        if (cart == null) return ApiResponseDto.FailureResponse("Cart not found");

        var response = new CartResponseDto
        {
            Id = cart.Id,
            CustomerId = cart.CustomerId,
            Items = cart.CartItems.Select(ci => new CartItemResponseDto
            {
                Id = ci.Id,
                ProductId = ci.ProductId,
                ProductName = ci.Product.Name,
                UnitPrice = ci.Product.Price,
                Quantity = ci.Quantity
            }).ToList()
        };

        return ApiResponseDto.SuccessResponse(response);
    }

    public async Task<ApiResponseDto> AddItemToCartAsync(int customerId, AddCartItemRequestDto request)
    {
        var cart = await _cartRepository.GetCartByCustomerIdAsync(customerId);
        if (cart == null)
        {
            cart = new Cart { CustomerId = customerId };
            await _cartRepository.AddAsync(cart);
            await _unitOfWork.SaveChangesAsync();
        }

        var cartItem = new CartItem
        {
            CartId = cart.Id,
            ProductId = request.ProductId,
            Quantity = request.Quantity
        };

        await _cartItemRepository.AddAsync(cartItem);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto.SuccessResponse(null, "Item added to cart");
    }

    public async Task<ApiResponseDto> UpdateCartItemAsync(int customerId, int cartItemId, UpdateCartItemRequestDto request)
    {
        var item = await _cartItemRepository.GetByIdAsync(cartItemId);
        if (item == null) return ApiResponseDto.FailureResponse("Item not found");
        
        item.Quantity = request.Quantity;
        _cartItemRepository.Update(item);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto.SuccessResponse(null, "Cart item updated");
    }

    public async Task<ApiResponseDto> RemoveCartItemAsync(int customerId, int cartItemId)
    {
        var item = await _cartItemRepository.GetByIdAsync(cartItemId);
        if (item == null) return ApiResponseDto.FailureResponse("Item not found");

        _cartItemRepository.Delete(item);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto.SuccessResponse(null, "Item removed from cart");
    }
}
