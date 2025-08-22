using System.Collections.Concurrent;
using Watch_API.Domain;

namespace Watch_API.Repositories;

public class InMemoryPortfolioRepository : IPortfolioRepository
{
    private readonly ConcurrentDictionary<Guid, Portfolio> _byId = new();


    public Task<List<Portfolio>> GetAllAsync(CancellationToken ct) => Task.FromResult(_byId.Values.OrderBy(a => a.Owner).ToList());
    public Task<Portfolio?> GetByIdAsync(Guid id, CancellationToken ct) => Task.FromResult(_byId.TryGetValue(id, out var p) ? p : null);
    public Task<Portfolio> AddAsync(Portfolio portfolio, CancellationToken ct) { _byId[portfolio.Id] = portfolio; return Task.FromResult(portfolio); }
    public Task<bool> UpdateAsync(Portfolio portfolio, CancellationToken ct) { if (!_byId.ContainsKey(portfolio.Id)) return Task.FromResult(false); _byId[portfolio.Id] = portfolio; return Task.FromResult(true); }
    public Task<bool> DeleteAsync(Guid id, CancellationToken ct) => Task.FromResult(_byId.TryRemove(id, out _));
}