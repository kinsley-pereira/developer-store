using Microsoft.EntityFrameworkCore;
using Sales.Domain.Entities;

namespace Sales.Application.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<Sale> Sales { get; }
        DbSet<SaleItem> SaleItems { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
