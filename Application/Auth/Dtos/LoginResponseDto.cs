namespace Ecommerce.Application.Auth.Dtos;

public record LoginResponseDto(
    string AccessToken,
    string RefreshToken,
    UserDto User
);
