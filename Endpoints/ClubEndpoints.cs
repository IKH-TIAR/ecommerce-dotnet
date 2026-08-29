using Ecommerce.Application.Clubs.Dtos;
using Ecommerce.Endpoints.Filters;

namespace Ecommerce.Endpoints;

public static class ClubEndpoints
{
    public static void MapClubEndpoints (this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/clubs");

        group.MapGet("/", ClubHandlers.GetAllClubs);
        group.MapPost("/", ClubHandlers.CreateClub).WithValidation<CreateClubDto>();
    }
}