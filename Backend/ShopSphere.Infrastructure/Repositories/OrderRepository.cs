using Microsoft.EntityFrameworkCore;
using ShopSphere.Application.Interfaces.Repositories;
using ShopSphere.Domain.Entities;
using ShopSphere.Infrastructure.Persistence;

namespace ShopSphere.Infrastructure.Repositories;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    public OrderRepository(ShopSphereDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(o => o.CustomerId == customerId).ToListAsync(cancellationToken);
    }
}
