using ShopSphere.Domain.Enums;

namespace ShopSphere.Application.DTOs.Auth;

public class RegisterRequestDto
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public UserRole Role { get; set; }
}

public class LoginRequestDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class AuthResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Token { get; set; } = string.Empty; // Placeholder if JWT is added later, or just for metadata
    public UserRole Role { get; set; }
}

public class CurrentUserResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public UserRole Role { get; set; }
}
