using ShopSphere.Application.Interfaces.Repositories;
using ShopSphere.Domain.Entities;
using ShopSphere.Infrastructure.Persistence;

namespace ShopSphere.Infrastructure.Repositories;

public class AdminProfileRepository : GenericRepository<AdminProfile>, IAdminProfileRepository
{
    public AdminProfileRepository(ShopSphereDbContext context) : base(context)
    {
    }
}
