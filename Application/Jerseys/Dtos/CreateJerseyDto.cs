namespace Ecommerce.Application.Jerseys.Dtos;

public record CreateJerseyDto(
    string Name,
    string? Description,
    List<string> ImageUrls,
    decimal Price,
    int StockQuantity,
    Guid ClubId
);