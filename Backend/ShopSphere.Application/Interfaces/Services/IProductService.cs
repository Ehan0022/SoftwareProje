using ShopSphere.Application.DTOs.Common;
using ShopSphere.Application.DTOs.Products;

namespace ShopSphere.Application.Interfaces.Services;

public interface IProductService
{
    Task<ApiResponseDto> GetAllProductsAsync();
    Task<ApiResponseDto> GetProductByIdAsync(int id);
    Task<ApiResponseDto> CreateProductAsync(int sellerId, ProductCreateRequestDto request);
    Task<ApiResponseDto> UpdateProductAsync(int sellerId, int productId, ProductUpdateRequestDto request);
    Task<ApiResponseDto> DeleteProductAsync(int sellerId, int productId);
}
