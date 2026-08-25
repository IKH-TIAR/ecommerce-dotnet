namespace Ecommerce.Domain.Entities;

public class Jersey
{
    public Guid Id {get; init;} = Guid.NewGuid();
    public required string Name {get; set;}
    public required string Club {get; set;}
    public string? Description {get; set;}
    public required List<string> ImageUrls {get; set;}
    public decimal Price {get; set;}
    public int StockQuantity {get; set;}
    public DateTimeOffset CreatedAt {get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt {get; set;}
}