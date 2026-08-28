namespace Ecommerce.Application.Jerseys.Dtos;

public record UpdateJerseyDto(
    string? Name,
    string? Description,
    List<string>? ImageUrls,
    decimal? Price,
    int? StockQuantity,
    Guid? ClubId
);