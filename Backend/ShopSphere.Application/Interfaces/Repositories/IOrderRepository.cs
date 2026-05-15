using ShopSphere.Domain.Entities;

namespace ShopSphere.Application.Interfaces.Repositories;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default);
}
