using ShopSphere.Application.DTOs.Common;

namespace ShopSphere.Application.Interfaces.Services;

public interface IUserService
{
    Task<ApiResponseDto> GetMyProfileAsync(int userId);
}
