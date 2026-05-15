using Microsoft.AspNetCore.Mvc;
using ShopSphere.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;

namespace ShopSphere.WebAPI.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var result = await _adminService.GetDashboardSummaryAsync();
        return Ok(result);
    }

    [HttpGet("orders")]
    public async Task<IActionResult> GetAllOrders()
    {
        var result = await _adminService.GetAllOrdersAsync();
        return Ok(result);
    }

    [HttpDelete("products/{id}")]
    public async Task<IActionResult> DeactivateProduct(int id)
    {
        var result = await _adminService.DeactivateProductAsync(id);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
