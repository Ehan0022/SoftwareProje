using Microsoft.EntityFrameworkCore;
using ShopSphere.Application.Interfaces.Repositories;
using ShopSphere.Domain.Entities;
using ShopSphere.Infrastructure.Persistence;

namespace ShopSphere.Infrastructure.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(ShopSphereDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetProductsBySellerIdAsync(int sellerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(p => p.SellerId == sellerId).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(p => p.IsActive).ToListAsync(cancellationToken);
    }
}
