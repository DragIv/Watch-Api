using Watch_API.Domain;

namespace Watch_API.Repositories;

public interface IPortfolioRepository
{
    Task<List<Portfolio>> GetAllAsync(CancellationToken ct);
    Task<Portfolio?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Portfolio> AddAsync(Portfolio portfolio, CancellationToken ct);
    Task<bool> UpdateAsync(Portfolio portfolio, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    
    // методы для работы с Holdings
    Task<Holding?> AddHoldingAsync(Guid portfolioId, string symbol, decimal amount, CancellationToken ct);
    Task<bool> RemoveHoldingAsync(Guid portfolioId, Guid holdingId, CancellationToken ct);
}