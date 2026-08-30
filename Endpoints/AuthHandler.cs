using Ecommerce.Application.Auth;
using Ecommerce.Application.Auth.Dtos;

namespace Ecommerce.Endpoints;

public static class AuthHandler
{
    public static async Task<IResult> Register(RegisterUserDto dto, IAuthService authService, CancellationToken ct)
    {
        var user = await authService.RegisterAsync(dto, ct);
        return Results.Created($"/api/auth/users/{user.Id}", user);
    }

    public static async Task<IResult> Login(LoginDto dto, IAuthService authService, CancellationToken ct)
    {
        var response = await authService.LoginAsync(dto, ct);
        return Results.Ok(response);
    }
}