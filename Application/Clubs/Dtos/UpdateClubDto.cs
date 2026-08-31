namespace Ecommerce.Application.Clubs.Dtos;

public record UpdateClubDto(
    string? Name,
    string? Country,
    string? League,
    string? LogoUrl
);
