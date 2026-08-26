namespace Ecommerce.Application.Jerseys.Dtos;

public record UpdateJerseyDto(
    string? Name,
    string? Club,
    string? Description,
    List<string>? ImageUrls,
    decimal? Price,
    int? StockQuantity
);