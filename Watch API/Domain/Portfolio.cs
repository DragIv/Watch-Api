namespace Watch_API.Domain;

public class Portfolio
{
    public Guid Id { get; set; } = Guid.NewGuid(); // при создании нового объекта - генерируется уникальный идентификатор
    public string Owner { get; set; } = "anonymous"; // если при создании объекта явно не указать владельца, то он будет анонимным
    public List<Holding> Holdings { get; set; } = new(); // у каждого нового портфеля будет список активов
}