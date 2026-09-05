using Ecommerce.Application.Clubs.Dtos;

namespace Ecommerce.Application.Jerseys.Dtos;

public record JerseyDto(
    Guid Id,
    string Name,
    string? Description,
    List<string> ImageUrls,
    decimal Price,
    int StockQuantity,
    Guid ClubId,
    ClubDto? Club,
    bool IsFeatured,
    bool IsTrending,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    List<JerseySizeStockDto> Sizes
);