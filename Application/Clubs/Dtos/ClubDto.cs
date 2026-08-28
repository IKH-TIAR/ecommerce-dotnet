namespace Ecommerce.Application.Clubs.Dtos;

public record ClubDto(
    Guid Id,
    string Name,
    string Country,
    string League,
    string? LogoUrl
);