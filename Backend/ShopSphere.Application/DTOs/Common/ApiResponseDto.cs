namespace ShopSphere.Application.DTOs.Common;

public class ApiResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }

    public static ApiResponseDto SuccessResponse(object? data = null, string message = "Operation successful")
        => new() { Success = true, Message = message, Data = data };

    public static ApiResponseDto FailureResponse(string message)
        => new() { Success = false, Message = message };
}
