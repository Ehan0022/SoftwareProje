using Microsoft.AspNetCore.Mvc;
using ShopSphere.Application.DTOs.Cart;
using ShopSphere.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ShopSphere.WebAPI.Controllers;

[Authorize(Roles = "Customer")]
[ApiController]
[Route("api/cart")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _cartService.GetCartAsync(customerId);
        return Ok(result);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem(AddCartItemRequestDto request)
    {
        var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _cartService.AddItemToCartAsync(customerId, request);
        return Ok(result);
    }

    [HttpPut("items/{cartItemId}")]
    public async Task<IActionResult> UpdateItem(int cartItemId, UpdateCartItemRequestDto request)
    {
        var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _cartService.UpdateCartItemAsync(customerId, cartItemId, request);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("items/{cartItemId}")]
    public async Task<IActionResult> RemoveItem(int cartItemId)
    {
        var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _cartService.RemoveCartItemAsync(customerId, cartItemId);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
