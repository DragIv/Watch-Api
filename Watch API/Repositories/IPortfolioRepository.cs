using Watch_API.Domain;

namespace Watch_API.Repositories;

public interface IPortfolioRepository
{
    Task<List<Portfolio>> GetAllAsync(CancellationToken ct);
    Task<Portfolio?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Portfolio> AddAsync(Portfolio portfolio, CancellationToken ct);
    Task<bool> UpdateAsync(Portfolio portfolio, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
}