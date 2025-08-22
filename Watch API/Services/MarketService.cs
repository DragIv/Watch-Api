using System.Text.Json;
using Watch_API.Domain;

namespace Watch_API.Services;

public class MarketService : IMarketService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MarketService> _logger;

    public MarketService(HttpClient httpClient, ILogger<MarketService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<PriceTick> GetQuoteAsync(string symbol)
    {
        try
        {
            var coinId = await GetCoinIdAsync(symbol);
            if (string.IsNullOrEmpty(coinId))
            {
                _logger.LogWarning("Could not find coingecko id for symbol {Symbol}", symbol);
                return null;
            }

            var url = $"https://api.coingecko.com/api/v3/simple/price?ids={coinId}&vs_currencies=usd";
            
            _logger.LogDebug("Fetching price for {Symbol} from сoingecko: {Url}", symbol, url);

            var response = await _httpClient.GetStringAsync(url);
            var data = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, decimal>>>(response);

            if (data != null && data.TryGetValue(coinId, out var priceData) && priceData.TryGetValue("usd", out var price))
            {
                var tick = new PriceTick 
                { 
                    Symbol = symbol.ToUpperInvariant(), 
                    PriceUsd = price, 
                    TimeUtc = DateTime.UtcNow 
                };
                
                _logger.LogInformation("Real price for {Symbol}: ${Price}", symbol, price);
                return tick;
            }
            else
            {
                _logger.LogWarning("No price data found for {Symbol} from CoinGecko API", symbol);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching price for {Symbol}", symbol);
            return null;
        }
    }

    public async Task<List<PriceTick>> GetTopAsync(int count)
    {
        try
        {
            var url = $"https://api.coingecko.com/api/v3/coins/markets" +
                      $"?vs_currency=usd&order=market_cap_desc&per_page={count}&page=1&sparkline=false";

            _logger.LogDebug("Requesting top {Count} coins from CoinGecko: {Url}", count, url);

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (compatible; MyCryptoApp/1.0)");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(content);

            if (document.RootElement.ValueKind != JsonValueKind.Array)
            {
                _logger.LogWarning("Expected JSON array but got: {Kind}", document.RootElement.ValueKind);
                return new List<PriceTick>();
            }

            var ticks = new List<PriceTick>();

            foreach (var coin in document.RootElement.EnumerateArray())
            {
                if (coin.TryGetProperty("symbol", out var symbolElement) &&
                    coin.TryGetProperty("current_price", out var priceElement))
                {
                    var symbol = symbolElement.GetString()?.ToUpperInvariant() ?? "";
                    var price = priceElement.GetDecimal();

                    ticks.Add(new PriceTick
                    {
                        Symbol = symbol,
                        PriceUsd = price,
                        TimeUtc = DateTime.UtcNow
                    });
                }
            }

            _logger.LogInformation("Parsed {Count} PriceTicks", ticks.Count);
            return ticks;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during GetTopAsync");
            return new List<PriceTick>();
        }
    }

    /// <summary>
    /// Ищет ID монеты по её символу через API CoinGecko.
    /// </summary>
    private async Task<string?> GetCoinIdAsync(string symbol)
    {
        try
        {
            var url = $"https://api.coingecko.com/api/v3/search?query={symbol}";
            var response = await _httpClient.GetStringAsync(url);
            using var document = JsonDocument.Parse(response);

            // Ищем точное совпадение символа в массиве "coins"
            foreach (var coin in document.RootElement.GetProperty("coins").EnumerateArray())
            {
                if (coin.TryGetProperty("symbol", out var symbolElement) &&
                    symbolElement.GetString()?.Equals(symbol, StringComparison.OrdinalIgnoreCase) == true)
                {
                    if (coin.TryGetProperty("id", out var idElement))
                    {
                        var coinId = idElement.GetString();
                        _logger.LogDebug("Found CoinGecko ID '{CoinId}' for symbol '{Symbol}'", coinId, symbol);
                        return coinId;
                    }
                }
            }
            return null; // ID не найден
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching for coin ID for symbol {Symbol}", symbol);
            return null;
        }
    }
}