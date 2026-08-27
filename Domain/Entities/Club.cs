namespace Ecommerce.Domain.Entities;

public class Club
{
    public Guid Id {get; init; } = Guid.NewGuid();
    public required string Name {get; set;}
    public required string Country {get; set;}
    public required string League {get; set;}
    public string? LogoUrl {get; set;}
    public DateTimeOffset CreatedAt {get; set;} = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt {get; set;}

    public List<Jersey> Jerseys {get; set;} = [];
}