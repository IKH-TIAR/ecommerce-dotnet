using Ecommerce.Application.Clubs;
using Ecommerce.Application.Clubs.Dtos;

namespace Ecommerce.Endpoints;

public static class ClubHandlers
{
    public static async Task<IResult> GetAllClubs(
        IClubService clubService, 
        int page = 1, 
        int pageSize = 10, 
        CancellationToken ct = default)
    {
        var clubs = await clubService.GetAllClubAsync(page, pageSize, ct);
        return Results.Ok(clubs);
    }

    public static async Task<IResult> GetClubById(
        Guid id, 
        IClubService clubService, 
        CancellationToken ct)
    {
        var club = await clubService.GetByIdAsync(id, ct);

        return club is null 
            ? Results.NotFound(new { message = $"Club with ID '{id}' was not found." }) 
            : Results.Ok(club);
    }

    public static async Task<IResult> CreateClub(
        CreateClubDto dto, 
        IClubService clubService, 
        CancellationToken ct)
    {
        var createdClub = await clubService.CreateClubAsync(dto, ct);
        return Results.Created($"/api/clubs/{createdClub.Id}", createdClub);
    }

    public static async Task<IResult> UpdateClub(
        Guid id, 
        UpdateClubDto dto, 
        IClubService clubService, 
        CancellationToken ct)
    {
        var updatedClub = await clubService.UpdateClubAsync(id, dto, ct);

        return updatedClub is null 
            ? Results.NotFound(new { message = $"Club with ID '{id}' was not found." }) 
            : Results.Ok(updatedClub);
    }

    public static async Task<IResult> DeleteClub(
        Guid id, 
        IClubService clubService, 
        CancellationToken ct)
    {
        var deleted = await clubService.DeleteClubAsync(id, ct);

        return !deleted 
            ? Results.NotFound(new { message = $"Club with ID '{id}' was not found." }) 
            : Results.NoContent();
    }
}