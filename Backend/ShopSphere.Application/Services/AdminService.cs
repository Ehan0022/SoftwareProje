using ShopSphere.Application.DTOs.Admin;
using ShopSphere.Application.DTOs.Common;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Interfaces.Repositories;
using ShopSphere.Application.Interfaces.Services;

namespace ShopSphere.Application.Services;

public class AdminService : IAdminService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AdminService(IOrderRepository orderRepository, IProductRepository productRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponseDto> GetDashboardSummaryAsync()
    {
        var orders = await _orderRepository.GetAllAsync();
        var products = await _productRepository.GetAllAsync();
        var users = await _userRepository.GetAllAsync();

        var response = new AdminDashboardResponseDto
        {
            TotalOrders = orders.Count,
            TotalProducts = products.Count,
            TotalCustomers = users.Count(u => u.Role == Domain.Enums.UserRole.Customer),
            TotalSellers = users.Count(u => u.Role == Domain.Enums.UserRole.Seller),
            TotalRevenue = orders.Sum(o => o.TotalAmount)
        };

        return ApiResponseDto.SuccessResponse(response);
    }

    public async Task<ApiResponseDto> GetAllOrdersAsync()
    {
        var orders = await _orderRepository.GetAllAsync();
        return ApiResponseDto.SuccessResponse(orders);
    }

    public async Task<ApiResponseDto> DeactivateProductAsync(int productId)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null) return ApiResponseDto.FailureResponse("Product not found");

        product.IsActive = false;
        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto.SuccessResponse(null, "Product deactivated successfully");
    }
}
