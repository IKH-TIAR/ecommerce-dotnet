using Ecommerce.Application.Clubs;
using Ecommerce.Application.Clubs.Dtos;

namespace Ecommerce.Endpoints;

public class ClubHandlers
{
    public static async Task<IResult> GetAllClubs(IClubService clubService, int page=1, int pageSize=10, CancellationToken ct = default)
    {

        var clubs = await clubService.GetAllClubAsync(page, pageSize, ct);
        return Results.Ok(clubs);
        
    }

    public static async Task<IResult> CreateClub (CreateClubDto dto, IClubService clubService, CancellationToken ct)
    {

        var createdClub = await clubService.CreateClubAsync(dto, ct);

        return Results.Created($"/api/clubs/{createdClub.Id}", createdClub);
    }
}