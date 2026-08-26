using Ecommerce.Application.Jerseys;

namespace Ecommerce.Endpoints;

public static class JerseyEndpoints
{
    public static void MapJerseyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/jerseys");

        group.MapGet("/", async (IJerseyService jerseyService, CancellationToken ct) =>
        {
            var jerseys = await jerseyService.GetAllAsync(ct);
            return Results.Ok(jerseys);
        });
    }
}