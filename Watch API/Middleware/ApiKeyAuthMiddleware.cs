namespace Watch_API.Middleware;

public class ApiKeyAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string? _key;


    public ApiKeyAuthMiddleware(RequestDelegate next, IConfiguration cfg)
    {
        _next = next;
        _key = cfg["Seed:ApiKey"];
    }


    public async Task InvokeAsync(HttpContext ctx)
    {
        // Разрешаем GET всем. Модифицирующие запросы к /api/portfolios требуют X-Api-Key
        if (ctx.Request.Path.StartsWithSegments("/api/portfolios") && ctx.Request.Method != HttpMethods.Get)
        {
            if (!ctx.Request.Headers.TryGetValue("X-Api-Key", out var provided) || string.IsNullOrEmpty(_key) || provided != _key)
            {
                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await ctx.Response.WriteAsync("Unauthorized");
                return;
            }
        }
        await _next(ctx);
    }
}