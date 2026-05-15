using Microsoft.EntityFrameworkCore;
using ShopSphere.Application.Interfaces.Repositories;
using ShopSphere.Domain.Entities;
using ShopSphere.Infrastructure.Persistence;

namespace ShopSphere.Infrastructure.Repositories;

public class CartRepository : GenericRepository<Cart>, ICartRepository
{
    public CartRepository(ShopSphereDbContext context) : base(context)
    {
    }

    public async Task<Cart?> GetCartByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.CustomerId == customerId, cancellationToken);
    }
}
