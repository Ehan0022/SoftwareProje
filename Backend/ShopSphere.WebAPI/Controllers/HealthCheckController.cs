using Microsoft.AspNetCore.Mvc;

namespace ShopSphere.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthCheckController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new 
        { 
            Status = "Healthy", 
            Message = "ShopSphere API is running", 
            Time = DateTime.UtcNow 
        });
    }
}
