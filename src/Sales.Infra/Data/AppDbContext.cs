using Microsoft.EntityFrameworkCore;
using Sales.Application.Interfaces;
using Sales.Domain.Entities;

namespace Sales.Infra.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
    {
        public DbSet<Sale> Sales { get; set; } = null!;
        public DbSet<SaleItem> SaleItems { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sale>(b =>
            {
                b.HasKey(s => s.Id);
                b.Property(s => s.SaleNumber).IsRequired();
                b.Property(s => s.CustomerName).HasMaxLength(200);
                b.Property(s => s.BranchName).HasMaxLength(200);

                b.HasMany(s => s.Items)
                 .WithOne(i => i.Sale)
                 .HasForeignKey(i => i.SaleId)
                 .IsRequired()
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<SaleItem>(b =>
            {
                b.HasKey(i => i.Id);
                b.Property(i => i.ProductName).HasMaxLength(200);
                b.Property(i => i.UnitPrice).HasPrecision(18, 2);
            });
        }
    }
}
