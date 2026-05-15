using ShopSphere.Application.Interfaces.Repositories;
using ShopSphere.Domain.Entities;
using ShopSphere.Infrastructure.Persistence;

namespace ShopSphere.Infrastructure.Repositories;

public class SellerProfileRepository : GenericRepository<SellerProfile>, ISellerProfileRepository
{
    public SellerProfileRepository(ShopSphereDbContext context) : base(context)
    {
    }
}
