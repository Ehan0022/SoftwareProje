using ShopSphere.Application.DTOs.Common;

namespace ShopSphere.Application.Interfaces.Services;

public interface IAdminService
{
    Task<ApiResponseDto> GetDashboardSummaryAsync();
    Task<ApiResponseDto> GetAllOrdersAsync();
    Task<ApiResponseDto> DeactivateProductAsync(int productId);
}
