using Ecommerce.Application.Common.Security;
using Ecommerce.Application.Jerseys.Dtos;
using Ecommerce.Endpoints.Filters;

namespace Ecommerce.Endpoints;

public static class JerseyEndpoints
{
    public static void MapJerseyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/jerseys");

        group.MapGet("/", JerseyHandlers.GetAllJerseys)
            .WithValidation<GetJerseysQuery>();

        group.MapGet("/{id:guid}", JerseyHandlers.GetById);

        group.MapPatch("/{id:guid}", JerseyHandlers.UpdateJersey)
            .WithValidation<UpdateJerseyDto>()
            .RequireAuthorization(Policies.AdminOnly);

        group.MapPost("/", JerseyHandlers.CreateJersey)
            .WithValidation<CreateJerseyDto>()
            .RequireAuthorization(Policies.AdminOnly);

        group.MapDelete("/{id:guid}", JerseyHandlers.DeleteJersey)
            .RequireAuthorization(Policies.AdminOnly);

        group.MapGet("/crash-test", () =>
        {
            throw new InvalidOperationException("Testing unhandled crash in GlobalExceptionHandler!");
        });
    }
}