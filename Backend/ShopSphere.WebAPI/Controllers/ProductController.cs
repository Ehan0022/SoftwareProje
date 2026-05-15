using Microsoft.AspNetCore.Mvc;
using ShopSphere.Application.DTOs.Products;
using ShopSphere.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ShopSphere.WebAPI.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _productService.GetAllProductsAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _productService.GetProductByIdAsync(id);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    [Authorize(Roles = "Seller")]
    [HttpPost]
    public async Task<IActionResult> Create(ProductCreateRequestDto request)
    {
        var sellerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _productService.CreateProductAsync(sellerId, request);
        return Ok(result);
    }

    [Authorize(Roles = "Seller")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ProductUpdateRequestDto request)
    {
        var sellerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _productService.UpdateProductAsync(sellerId, id, request);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [Authorize(Roles = "Seller")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var sellerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _productService.DeleteProductAsync(sellerId, id);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }
}
