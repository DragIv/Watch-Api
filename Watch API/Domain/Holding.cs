namespace Watch_API.Domain;

public class Holding
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Symbol { get; set; } = string.Empty; // BTC
    public decimal Amount { get; set; } // 0.5
    
    // навигационные свойства для EF Core
    public Guid PortfolioId { get; set; }
    public Portfolio Portfolio { get; set; } = null!;
}