using ShopSphere.Application.DTOs.Auth;
using ShopSphere.Application.DTOs.Common;

namespace ShopSphere.Application.Interfaces.Services;

public interface IAuthService
{
    Task<ApiResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<ApiResponseDto> LoginAsync(LoginRequestDto request);
    Task<ApiResponseDto> GetCurrentUserAsync(int userId);
    Task<ApiResponseDto> LogoutAsync();
}
