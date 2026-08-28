namespace Ecommerce.Application.Clubs.Dtos;

public record CreateClubDto(
    string Name,
    string Country,
    string League,
    String? LogoUrl
);