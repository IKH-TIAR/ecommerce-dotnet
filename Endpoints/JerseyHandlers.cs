using Ecommerce.Application.Jerseys;
using Ecommerce.Application.Jerseys.Dtos;
using FluentValidation;

namespace Ecommerce.Endpoints;

public class JerseyHandlers
{
    public static async Task<IResult> GetAllJerseys(IJerseyService jerseyService, CancellationToken ct)
    {
        var jerseys = await jerseyService.GetAllAsync(ct);
        return Results.Ok(jerseys);
    }

    public static async Task<IResult> GetById (Guid Id, IJerseyService jerseyService, CancellationToken ct)
    {
        var jersey = await jerseyService.GetByIdAsync(Id, ct);

        return jersey is null 
        ? Results.NotFound()
        : Results.Ok(jersey);
    }

    public static async Task<IResult> CreateJersey(CreateJerseyDto dto, IValidator<CreateJerseyDto> validator, IJerseyService jerseyService, CancellationToken ct)
    {
        var validationResult = await validator.ValidateAsync(dto, ct);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }
        var createdJersey = await jerseyService.CreateJerseyAsync(dto, ct);

        return Results.Created($"/api/jerseys/{createdJersey.Id}", createdJersey);
    }
}