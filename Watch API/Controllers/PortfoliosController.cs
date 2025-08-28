using Microsoft.AspNetCore.Mvc;
using Watch_API.Domain;
using Watch_API.DTO;
using Watch_API.Repositories;

namespace Watch_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PortfoliosController : ControllerBase
{
    private readonly IPortfolioRepository _repo;

    public PortfoliosController(IPortfolioRepository repo) => _repo = repo;

    /// <summary>
    /// Получить все портфели
    /// </summary>
    /// <returns>Список всех портфелей с кратким описанием</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<PortfolioDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var items = await _repo.GetAllAsync(ct);
        return Ok(items.Select(p => new PortfolioDto(p.Id, p.Owner, p.Holdings.Count)).ToList());
    }
    
    /// <summary>
    /// Получить детали портфеля
    /// </summary>
    /// <param name="id">ID портфеля</param>
    /// <returns>Подробная информация о портфеле со всеми позициями</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PortfolioDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var p = await _repo.GetByIdAsync(id, ct);
        if (p is null) return NotFound();
        return Ok(new PortfolioDetailsDto(p.Id, p.Owner, p.Holdings.Select(h => new HoldingDto(h.Id, h.Symbol, h.Amount)).ToList()));
    }

    /// <summary>
    /// Создать новый портфель
    /// </summary>
    /// <param name="dto">Данные владельца портфеля</param>
    [HttpPost]
    [ProducesResponseType(typeof(PortfolioDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreatePortfolioDto dto, CancellationToken ct)
    {
        var p = new Portfolio { Owner = dto.Owner };
        await _repo.AddAsync(p, ct);
        return CreatedAtAction(nameof(Get), new { id = p.Id }, new PortfolioDto(p.Id, p.Owner, 0));
    }

    /// <summary>
    /// Обновить информацию о портфеле (имя)
    /// </summary>
    /// <param name="id">ID портфеля для обновления</param>
    /// <param name="dto">Новые данные портфеля (владелец)</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePortfolioDto dto, CancellationToken ct)
    {
        var p = await _repo.GetByIdAsync(id, ct);
        if (p is null) return NotFound();
        p.Owner = dto.Owner;
        return await _repo.UpdateAsync(p, ct) ? NoContent() : NotFound();
    }
    
    /// <summary>
    /// Удалить весь портфель
    /// </summary>
    /// <param name="id">ID портфеля для удаления</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => (await _repo.DeleteAsync(id, ct)) ? NoContent() : NotFound();

    /// <summary>
    /// Добавить криптовалюту в портфель (купить)
    /// </summary>
    /// <param name="portfolioId">ID портфеля</param>
    /// <param name="dto">Какую криптовалюту и сколько добавить</param>
    /// <returns>Созданная позиция в портфеле</returns>
    [HttpPost("{portfolioId:guid}/holdings")]
    [ProducesResponseType(typeof(HoldingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddHolding(Guid portfolioId, [FromBody] AddHoldingDto dto, CancellationToken ct)
    {
        var holding = await _repo.AddHoldingAsync(portfolioId, dto.Symbol.ToUpperInvariant(), dto.Amount, ct);
        if (holding == null) 
        {
            return NotFound($"Portfolio with ID {portfolioId} not found");
        }
        
        return CreatedAtAction(nameof(Get), new { id = portfolioId }, 
            new HoldingDto(holding.Id, holding.Symbol, holding.Amount));
    }
    
    /// <summary>
    /// Удалить криптовалюту из портфеля (продать все)
    /// </summary>
    /// <param name="portfolioId">ID портфеля</param>
    /// <param name="holdingId">ID позиции для удаления</param>
    [HttpDelete("{portfolioId:guid}/holdings/{holdingId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteHolding(Guid portfolioId, Guid holdingId, CancellationToken ct)
    {
        var success = await _repo.RemoveHoldingAsync(portfolioId, holdingId, ct);
        return success ? NoContent() : NotFound();
    }
}