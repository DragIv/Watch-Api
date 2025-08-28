using Microsoft.EntityFrameworkCore;
using Watch_API.Domain;
using Watch_API.EFCore;

namespace Watch_API.Repositories;

public class EfPortfolioRepository : IPortfolioRepository
{
    private readonly CryptoDbContext _context;
    private readonly ILogger<EfPortfolioRepository> _logger;

    public EfPortfolioRepository(CryptoDbContext context, ILogger<EfPortfolioRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<Portfolio>> GetAllAsync(CancellationToken ct)
    {
        return await _context.Portfolios
            .Include(p => p.Holdings)
            .OrderBy(p => p.Owner)
            .ToListAsync(ct);
    }

    public async Task<Portfolio?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.Portfolios
            .Include(p => p.Holdings)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<Portfolio> AddAsync(Portfolio portfolio, CancellationToken ct)
    {
        _context.Portfolios.Add(portfolio);
        await _context.SaveChangesAsync(ct);
        
        _logger.LogInformation("Added portfolio {PortfolioId} for owner {Owner}", 
            portfolio.Id, portfolio.Owner);
        
        return portfolio;
    }

    public async Task<bool> UpdateAsync(Portfolio portfolio, CancellationToken ct)
    {
        var existingPortfolio = await _context.Portfolios
            .Include(p => p.Holdings)
            .FirstOrDefaultAsync(p => p.Id == portfolio.Id, ct);
            
        if (existingPortfolio == null)
        {
            return false;
        }

        // Обновляем основные свойства портфеля
        existingPortfolio.Owner = portfolio.Owner;

        // Синхронизируем Holdings
        // Удаляем Holdings, которых нет в новом портфеле
        var holdingsToRemove = existingPortfolio.Holdings
            .Where(existing => !portfolio.Holdings.Any(updated => updated.Id == existing.Id))
            .ToList();
        
        foreach (var holding in holdingsToRemove)
        {
            _context.Holdings.Remove(holding);
        }

        // Добавляем или обновляем Holdings
        foreach (var holding in portfolio.Holdings)
        {
            var existingHolding = existingPortfolio.Holdings
                .FirstOrDefault(h => h.Id == holding.Id);
                
            if (existingHolding != null)
            {
                // Обновляем существующий
                existingHolding.Symbol = holding.Symbol;
                existingHolding.Amount = holding.Amount;
            }
            else
            {
                // Добавляем новый
                holding.PortfolioId = portfolio.Id;
                existingPortfolio.Holdings.Add(holding);
            }
        }

        await _context.SaveChangesAsync(ct);
        
        _logger.LogInformation("Updated portfolio {PortfolioId}", portfolio.Id);
        
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var portfolio = await _context.Portfolios.FindAsync(id);
        if (portfolio == null)
        {
            return false;
        }

        _context.Portfolios.Remove(portfolio);
        await _context.SaveChangesAsync(ct);
        
        _logger.LogInformation("Deleted portfolio {PortfolioId}", id);
        
        return true;
    }
    
    
    public async Task<Holding?> AddHoldingAsync(Guid portfolioId, string symbol, decimal amount, CancellationToken ct)
    {
        var portfolio = await _context.Portfolios.FindAsync(portfolioId);
        if (portfolio == null)
        {
            return null;
        }

        var holding = new Holding 
        { 
            Symbol = symbol, 
            Amount = amount, 
            PortfolioId = portfolioId 
        };

        _context.Holdings.Add(holding);
        await _context.SaveChangesAsync(ct);
        
        _logger.LogInformation("Added holding {Symbol} ({Amount}) to portfolio {PortfolioId}", 
            symbol, amount, portfolioId);
        
        return holding;
    }

    public async Task<bool> RemoveHoldingAsync(Guid portfolioId, Guid holdingId, CancellationToken ct)
    {
        var holding = await _context.Holdings
            .FirstOrDefaultAsync(h => h.Id == holdingId && h.PortfolioId == portfolioId, ct);
            
        if (holding == null)
        {
            return false;
        }

        _context.Holdings.Remove(holding);
        await _context.SaveChangesAsync(ct);
        
        _logger.LogInformation("Removed holding {HoldingId} from portfolio {PortfolioId}", 
            holdingId, portfolioId);
        
        return true;
    }
    
}