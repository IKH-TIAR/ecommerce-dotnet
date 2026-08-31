using Ecommerce.Application.Clubs.Dtos;
using Ecommerce.Application.Common.Security;
using Ecommerce.Endpoints.Filters;

namespace Ecommerce.Endpoints;

public static class ClubEndpoints
{
    public static void MapClubEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/clubs");

        // Public Endpoints
        group.MapGet("/", ClubHandlers.GetAllClubs);
        group.MapGet("/{id:guid}", ClubHandlers.GetClubById);

        // Protected Endpoints (Admin Only)
        group.MapPost("/", ClubHandlers.CreateClub)
            .WithValidation<CreateClubDto>()
            .RequireAuthorization(Policies.AdminOnly);

        group.MapPatch("/{id:guid}", ClubHandlers.UpdateClub)
            .WithValidation<UpdateClubDto>()
            .RequireAuthorization(Policies.AdminOnly);

        group.MapDelete("/{id:guid}", ClubHandlers.DeleteClub)
            .RequireAuthorization(Policies.AdminOnly);
    }
}