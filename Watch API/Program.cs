using System.Reflection;
using Microsoft.OpenApi.Models;
using Serilog;
using Watch_API.Domain;
using Watch_API.Logging;
using Watch_API.Middleware;
using Watch_API.Minimal;
using Watch_API.Repositories;
using Watch_API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogLogging();

// DI registrations
builder.Services.AddSingleton<IPortfolioRepository, InMemoryPortfolioRepository>();
builder.Services.AddSingleton<IMarketService, MarketService>();
builder.Services.AddHttpClient<IMarketService, MarketService>();

builder.Services.AddControllers();


// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer(); // Нужно, чтобы Swagger знал, какие контроллеры и методы есть
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CryptoWatch API",
        Version = "v1",
        Description = "API для управления криптовалютными портфелями и получения рыночных котировок. " +
                      "Позволяет создавать портфели, отслеживать позиции и получать актуальные цены криптовалют."
    });
    
    // Включить XML комментарии
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath)) 
        c.IncludeXmlComments(xmlPath);
});
var app = builder.Build();


// Middleware pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>(); // логируем исключения 
app.UseSerilogRequestLogging(); // логирует HTTP запросы/ответы
app.UseMiddleware<RequestTimingMiddleware>(); // измеряем время запроса


// простая аутентификация/авторизация API Key
// app.UseMiddleware<ApiKeyAuthMiddleware>();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapControllers();
app.MapMarket();
app.MapDashboard();


// Demo seed
if (app.Configuration.GetValue<bool>("Seed:EnableDemoSeed"))
{
    
    var portfolios = app.Services.GetRequiredService<IPortfolioRepository>();
    await portfolios.AddAsync(new Watch_API.Domain.Portfolio
    {
        Owner = "demo",
        Holdings = new()
        {
            new Watch_API.Domain.Holding{ Symbol = "BTC", Amount = 0.2m },
            new Watch_API.Domain.Holding{ Symbol = "ETH", Amount = 1.5m }
        }
    }, default);


    Log.Information("Seeded demo data");
}


app.Run();