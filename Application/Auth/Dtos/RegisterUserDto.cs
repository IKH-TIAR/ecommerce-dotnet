namespace Ecommerce.Application.Auth.Dtos;

public record RegisterUserDto(
    string FullName,
    string Email,
    string Password
);