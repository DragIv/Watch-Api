namespace Watch_API.DTO;

public record PortfolioDto(Guid Id, string Owner, int Positions);
public record PortfolioDetailsDto(Guid Id, string Owner, List<HoldingDto> Holdings);
public record HoldingDto(Guid Id, string Symbol, decimal Amount);
public record CreatePortfolioDto(string Owner);
public record UpdatePortfolioDto(string Owner);
public record AddHoldingDto(string Symbol, decimal Amount);

public record UpdateHoldingDto(decimal Amount);