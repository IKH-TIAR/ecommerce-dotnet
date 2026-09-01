namespace Ecommerce.Application.Jerseys.Dtos;

public record CreateJerseyDto(
    string Name,
    string? Description,
    List<string> ImageUrls,
    decimal Price,
    Guid ClubId,
    List<JerseySizeStockDto> Sizes
);