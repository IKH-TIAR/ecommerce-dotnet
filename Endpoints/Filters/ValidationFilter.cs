using FluentValidation;

namespace Ecommerce.Endpoints.Filters;


public class ValidationFilter<T>(IValidator<T>? validator = null) : IEndpointFilter where T : class
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next
    )
    {
        if (validator is null)
        {
            return await next(context);
        }
        var argument = context.Arguments.OfType<T>().FirstOrDefault();

        if (argument is null)
        {
            return Results.BadRequest(
                new
                {
                    message = "Request body cannot be empty"
                }
            );
        }

        var validationResult = await validator.ValidateAsync(argument, context.HttpContext.RequestAborted);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        return await next(context);

    }
}
