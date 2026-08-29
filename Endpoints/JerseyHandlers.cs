using Ecommerce.Application.Jerseys;
using Ecommerce.Application.Jerseys.Dtos;
using FluentValidation;

namespace Ecommerce.Endpoints;

public class JerseyHandlers
{
    public static async Task<IResult> GetAllJerseys(IJerseyService jerseyService, int page = 1, int pageSize = 10, CancellationToken ct = default)
    {
        var jerseys = await jerseyService.GetAllAsync(page, pageSize, ct);
        return Results.Ok(jerseys);
    }

    public static async Task<IResult> GetById (Guid Id, IJerseyService jerseyService, CancellationToken ct)
    {
        var jersey = await jerseyService.GetByIdAsync(Id, ct);

        return jersey is null 
        ? Results.NotFound()
        : Results.Ok(jersey);
    }

    public static async Task<IResult> CreateJersey(CreateJerseyDto dto, IJerseyService jerseyService, CancellationToken ct)
    {
        var createdJersey = await jerseyService.CreateJerseyAsync(dto, ct);

        return Results.Created($"/api/jerseys/{createdJersey.Id}", createdJersey);
    }

    public static async Task<IResult> UpdateJersey(Guid id,IJerseyService jerseyService, UpdateJerseyDto dto, CancellationToken ct)
    {

        var updatedJersey = await jerseyService.UpdateJerseyAsync(id, dto, ct);

        return updatedJersey is null
        ? Results.NotFound(new
        {
            message = $"Jersey With ID {id} was not found"
        })
        : Results.Ok(updatedJersey);
    }

    public static async Task<IResult> DeleteJersey(Guid id, IJerseyService jerseyService, CancellationToken ct)
    {
        var deleteResult = await jerseyService.DeleteJerseyAsync(id, ct);

        return !deleteResult 
        ? Results.NotFound()
        : Results.NoContent(); 
    }
}