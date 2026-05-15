using ShopSphere.Domain.Entities;

namespace ShopSphere.Application.Interfaces.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<IEnumerable<Product>> GetProductsBySellerIdAsync(int sellerId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default);
}
