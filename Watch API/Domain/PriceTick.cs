namespace Watch_API.Domain;

public class PriceTick
{
    public string Symbol { get; set; } = string.Empty;
    public decimal PriceUsd { get; set; }
    public DateTime TimeUtc { get; set; } = DateTime.UtcNow;
}