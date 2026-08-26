using Ecommerce.Application.Jerseys;

namespace Ecommerce.Endpoints;

public static class JerseyEndpoints
{
    public static void MapJerseyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/jerseys");

        group.MapGet("/", JerseyHandlers.GetAllJerseys);
        group.MapPost("/", JerseyHandlers.CreateJersey);

    }
}