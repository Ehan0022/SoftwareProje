using Microsoft.AspNetCore.Mvc;
using ShopSphere.Application.DTOs.Orders;
using ShopSphere.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ShopSphere.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [Authorize(Roles = "Customer")]
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout(CheckoutRequestDto request)
    {
        var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _orderService.CheckoutAsync(customerId, request);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [Authorize(Roles = "Customer")]
    [HttpGet("my-orders")]
    public async Task<IActionResult> MyOrders()
    {
        var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _orderService.GetMyOrdersAsync(customerId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var isAdmin = User.IsInRole("Admin");
        
        var result = await _orderService.GetOrderByIdAsync(userId, id, isAdmin);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }
}
