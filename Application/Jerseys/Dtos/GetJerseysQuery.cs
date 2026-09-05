namespace Ecommerce.Application.Jerseys.Dtos;

public record GetJerseysQuery(
    string? SearchTerm = null,
    Guid? ClubId = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    bool? InStockOnly = null,
    bool? IsFeatured = null,
    bool? IsTrending = null,
    bool? IsNewArrival = null,
    string? SortBy = null,
    string? SortOrder = "desc",
    int Page = 1,
    int PageSize = 10
);
