using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Watch_API.Domain;
using Watch_API.Services;

namespace Watch_API.Minimal;

public static class MarketMinimalApi
{
    public static RouteGroupBuilder MapMarket(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/market").WithTags("Market");

        group.MapGet("/quote/{symbol}", async (string symbol, IMarketService market) =>
            {
                var quote = await market.GetQuoteAsync(symbol);

                if (quote == null)
                {
                    return Results.NotFound($"Криптовалюта с символом '{symbol}' не найдена.");
                }

                return Results.Ok(quote);
            })
            .WithName("GetQuote")
            .WithSummary("Получить реальную цену криптовалюты")
            .WithDescription("Возвращает актуальную цену из CoinGecko API")
            .Produces<PriceTick>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/top", async (IMarketService market) =>
            {
                var prices = await market.GetTopAsync(20); // Топ 20
                return Results.Ok(prices);
            })
            .WithName("GetTop")
            .WithSummary("Получить реальные цены топ-20 криптовалют")
            .WithDescription("Возвращает цены топ-20 монет по рыночной капитализации из CoinGecko")
            .Produces<List<PriceTick>>(StatusCodes.Status200OK);

        return group;
    }
}