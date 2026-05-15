using ShopSphere.Application.DTOs.Auth;
using ShopSphere.Application.DTOs.Common;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Interfaces.Repositories;
using ShopSphere.Application.Interfaces.Services;
using ShopSphere.Domain.Entities;

namespace ShopSphere.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null) return ApiResponseDto.FailureResponse("Email already exists.");

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = request.Password, // Should be hashed in production
            Role = request.Role
        };

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto.SuccessResponse(null, "Registration successful");
    }

    public async Task<ApiResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null || user.PasswordHash != request.Password)
        {
            return ApiResponseDto.FailureResponse("Invalid credentials.");
        }

        var response = new AuthResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role
        };

        return ApiResponseDto.SuccessResponse(response, "Login successful");
    }

    public async Task<ApiResponseDto> GetCurrentUserAsync(int userId)
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

    public async Task<ApiResponseDto> LogoutAsync()
    {
        return await Task.FromResult(ApiResponseDto.SuccessResponse(null, "Logged out successfully"));
    }
}
