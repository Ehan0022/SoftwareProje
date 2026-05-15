using ShopSphere.Application.Interfaces;
using ShopSphere.Infrastructure.Persistence;

namespace ShopSphere.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly ShopSphereDbContext _context;

    public UnitOfWork(ShopSphereDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
