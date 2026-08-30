namespace Ecommerce.Application.Auth.Dtos;


public record LoginDto(
    string Email,
    string Password
);