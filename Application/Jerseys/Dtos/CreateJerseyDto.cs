namespace Ecommerce.Application.Jerseys.Dtos;

public record CreateJerseyDto(
    string Name,
    string Club,
    string? Description,
    List<string> ImageUrls,
    decimal Price,
    int StockQuantity

);