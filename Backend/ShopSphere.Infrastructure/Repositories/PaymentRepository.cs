using ShopSphere.Application.Interfaces.Repositories;
using ShopSphere.Domain.Entities;
using ShopSphere.Infrastructure.Persistence;

namespace ShopSphere.Infrastructure.Repositories;

public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(ShopSphereDbContext context) : base(context)
    {
    }
}
