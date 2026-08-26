namespace Ecommerce.Application.Jerseys.Dtos;

public record JerseyDto(
    Guid Id,
    string Name,
    string Club,
    string? Description,
    List<string> ImageUrls,
    decimal Price,
    int StockQuantity,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt

);