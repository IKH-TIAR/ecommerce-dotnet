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
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt

);