using Watch_API.Domain;

namespace Watch_API.Services;

public interface IMarketService
{
    Task<PriceTick> GetQuoteAsync(string symbol); // Получить цену одной монеты
    Task<List<PriceTick>> GetTopAsync(int count); // Получить топ N монет
}