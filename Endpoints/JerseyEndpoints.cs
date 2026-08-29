using Ecommerce.Application.Jerseys;
using Ecommerce.Application.Jerseys.Dtos;
using Ecommerce.Endpoints.Filters;

namespace Ecommerce.Endpoints;

public static class JerseyEndpoints
{
    public static void MapJerseyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/jerseys");

        group.MapGet("/", JerseyHandlers.GetAllJerseys);
        group.MapGet("/{id:guid}", JerseyHandlers.GetById);
        group.MapPatch("/{id:guid}", JerseyHandlers.UpdateJersey).WithValidation<UpdateJerseyDto>();
        group.MapPost("/", JerseyHandlers.CreateJersey).WithValidation<CreateJerseyDto>();
        group.MapDelete("/{id:guid}", JerseyHandlers.DeleteJersey);

        group.MapGet("/crash-test", () =>
        {
            throw new InvalidOperationException("Testing unhandled crash in GlobalExceptionHandler!");
        });
    }
}