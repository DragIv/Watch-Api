using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Watch_API.Domain;
using Watch_API.EFCore;
using Watch_API.Logging;
using Watch_API.Middleware;
using Watch_API.Minimal;
using Watch_API.Repositories;
using Watch_API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogLogging();

// Entity Framework с PostgreSQL
builder.Services.AddDbContext<CryptoDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// DI registrations
builder.Services.AddScoped<IPortfolioRepository, EfPortfolioRepository>();
builder.Services.AddSingleton<IMarketService, MarketService>();
builder.Services.AddHttpClient<IMarketService, MarketService>();

builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
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

// Автоматическая миграция БД при запуске
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CryptoDbContext>();
    try
    {
        await context.Database.MigrateAsync();
        Log.Information("Database migration completed successfully");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Database migration failed");
        throw;
    }
}

// Middleware pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSerilogRequestLogging();
app.UseMiddleware<RequestTimingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.MapMarket();
app.MapDashboard();

// Demo seed - теперь работает с БД
if (app.Configuration.GetValue<bool>("Seed:EnableDemoSeed"))
{
    using var scope = app.Services.CreateScope();
    var portfolios = scope.ServiceProvider.GetRequiredService<IPortfolioRepository>();
    var context = scope.ServiceProvider.GetRequiredService<CryptoDbContext>();
    
    // Проверяем, есть ли уже данные
    if (!await context.Portfolios.AnyAsync())
    {
        await portfolios.AddAsync(new Portfolio
        {
            Owner = "demo",
            Holdings = new()
            {
                new Holding{ Symbol = "BTC", Amount = 0.2m },
                new Holding{ Symbol = "ETH", Amount = 1.5m }
            }
        }, default);

        Log.Information("Seeded demo data to database");
    }
    else
    {
        Log.Information("Demo data already exists, skipping seed");
    }
}

app.Run();