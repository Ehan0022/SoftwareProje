using ShopSphere.Application.DTOs.Auth;
using ShopSphere.Application.DTOs.Common;
using ShopSphere.Application.Interfaces.Repositories;
using ShopSphere.Application.Interfaces.Services;

namespace ShopSphere.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ApiResponseDto> GetMyProfileAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return ApiResponseDto.FailureResponse("User not found");

        var response = new CurrentUserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role
        };

        return ApiResponseDto.SuccessResponse(response);
    }
}
