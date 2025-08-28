using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Watch_API.Domain;

namespace Watch_API.Configurations;

public class PortfolioConfiguration : IEntityTypeConfiguration<Portfolio>
{
    public void Configure(EntityTypeBuilder<Portfolio> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Owner)
            .HasMaxLength(100)
            .IsRequired();
            
        // Настройка связи один-ко-многим с Holdings
        builder.HasMany(p => p.Holdings)
            .WithOne(h => h.Portfolio)
            .HasForeignKey(h => h.PortfolioId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Настройка таблицы
        builder.ToTable("Portfolios");
    }
}