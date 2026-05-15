using ShopSphere.Application.DTOs.Common;
using ShopSphere.Application.DTOs.Products;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Interfaces.Repositories;
using ShopSphere.Application.Interfaces.Services;
using ShopSphere.Domain.Entities;

namespace ShopSphere.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponseDto> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        var response = products.Select(p => new ProductResponseDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            IsActive = p.IsActive,
            SellerId = p.SellerId,
            CategoryId = p.CategoryId
        }).ToList();

        return ApiResponseDto.SuccessResponse(response);
    }

    public async Task<ApiResponseDto> GetProductByIdAsync(int id)
    {
        var p = await _productRepository.GetByIdAsync(id);
        if (p == null) return ApiResponseDto.FailureResponse("Product not found");

        var response = new ProductResponseDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            IsActive = p.IsActive,
            SellerId = p.SellerId,
            CategoryId = p.CategoryId
        };

        return ApiResponseDto.SuccessResponse(response);
    }

    public async Task<ApiResponseDto> CreateProductAsync(int sellerId, ProductCreateRequestDto request)
    {
        var product = new Product
        {
            SellerId = sellerId,
            CategoryId = request.CategoryId,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            IsActive = true
        };

        await _productRepository.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto.SuccessResponse(product.Id, "Product created successfully");
    }

    public async Task<ApiResponseDto> UpdateProductAsync(int sellerId, int productId, ProductUpdateRequestDto request)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null) return ApiResponseDto.FailureResponse("Product not found");
        if (product.SellerId != sellerId) return ApiResponseDto.FailureResponse("You can only manage your own products");

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.StockQuantity = request.StockQuantity;
        product.IsActive = request.IsActive;

        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto.SuccessResponse(null, "Product updated successfully");
    }

    public async Task<ApiResponseDto> DeleteProductAsync(int sellerId, int productId)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null) return ApiResponseDto.FailureResponse("Product not found");
        if (product.SellerId != sellerId) return ApiResponseDto.FailureResponse("You can only manage your own products");

        _productRepository.Delete(product);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto.SuccessResponse(null, "Product deleted successfully");
    }
}
