using ShopSphere.Application.Interfaces.Repositories;
using ShopSphere.Domain.Entities;
using ShopSphere.Infrastructure.Persistence;

namespace ShopSphere.Infrastructure.Repositories;

public class CartItemRepository : GenericRepository<CartItem>, ICartItemRepository
{
    public CartItemRepository(ShopSphereDbContext context) : base(context)
    {
    }
}
