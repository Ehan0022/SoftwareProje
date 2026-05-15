using ShopSphere.Application.Interfaces.Repositories;
using ShopSphere.Domain.Entities;
using ShopSphere.Infrastructure.Persistence;

namespace ShopSphere.Infrastructure.Repositories;

public class CustomerProfileRepository : GenericRepository<CustomerProfile>, ICustomerProfileRepository
{
    public CustomerProfileRepository(ShopSphereDbContext context) : base(context)
    {
    }
}
