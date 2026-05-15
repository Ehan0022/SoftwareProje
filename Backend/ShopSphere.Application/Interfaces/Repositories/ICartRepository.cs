using ShopSphere.Domain.Entities;

namespace ShopSphere.Application.Interfaces.Repositories;

public interface ICartRepository : IGenericRepository<Cart>
{
    Task<Cart?> GetCartByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default);
}
