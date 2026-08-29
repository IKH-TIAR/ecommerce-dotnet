using Ecommerce.Application.Auth.Dtos;
using Ecommerce.Endpoints.Filters;

namespace Ecommerce.Endpoints;

public static class AuthEndpoint
{
    public static void MapAuthEndpoints (this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/register", AuthHandler.Register).WithValidation<RegisterUserDto>();
    }
}