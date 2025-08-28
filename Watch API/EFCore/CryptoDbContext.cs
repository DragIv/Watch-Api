using Microsoft.EntityFrameworkCore;
using Watch_API.Domain;

namespace Watch_API.EFCore;

public class CryptoDbContext : DbContext
{
    public DbSet<Portfolio> Portfolios { get; set; }
    public DbSet<Holding> Holdings { get; set; }

    public CryptoDbContext(DbContextOptions<CryptoDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Применяем все конфигурации из текущей сборки
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CryptoDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
}