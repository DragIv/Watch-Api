using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Watch_API.Domain;

namespace Watch_API.Configurations;

public class HoldingConfiguration : IEntityTypeConfiguration<Holding>
{
    public void Configure(EntityTypeBuilder<Holding> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Symbol)
            .HasMaxLength(10)
            .IsRequired();
            
        builder.Property(e => e.Amount)
            .HasColumnType("decimal(18,8)")
            .IsRequired();
            
        builder.Property(e => e.PortfolioId)
            .IsRequired();
            
        // Настройка таблицы
        builder.ToTable("Holdings");
        
        // Индекс для быстрого поиска по символу
        builder.HasIndex(h => h.Symbol)
            .HasDatabaseName("IX_Holdings_Symbol");
            
        // Композитный индекс для поиска по портфелю + символу
        builder.HasIndex(h => new { h.PortfolioId, h.Symbol })
            .HasDatabaseName("IX_Holdings_Portfolio_Symbol");
    }
}