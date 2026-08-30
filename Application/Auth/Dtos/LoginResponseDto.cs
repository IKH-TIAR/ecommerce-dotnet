namespace Ecommerce.Application.Auth.Dtos;


public record LoginResponseDto(
    string Token,
    UserDto User
);
