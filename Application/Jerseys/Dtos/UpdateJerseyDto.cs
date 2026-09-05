namespace Ecommerce.Application.Jerseys.Dtos;

public record UpdateJerseyDto(
    string? Name,
    string? Description,
    List<string>? ImageUrls,
    decimal? Price,
    Guid? ClubId,
    List<JerseySizeStockDto>? Sizes = null,
    bool? IsFeatured = null,
    bool? IsTrending = null
);