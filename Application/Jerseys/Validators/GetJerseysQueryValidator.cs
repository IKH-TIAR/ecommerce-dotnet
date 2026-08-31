using Ecommerce.Application.Jerseys.Dtos;
using FluentValidation;

namespace Ecommerce.Application.Jerseys.Validators;

public class GetJerseysQueryValidator : AbstractValidator<GetJerseysQuery>
{
    private static readonly string[] AllowedSortColumns = ["price", "name", "createdat"];
    private static readonly string[] AllowedSortOrders = ["asc", "desc"];

    public GetJerseysQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page number must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");

        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum price cannot be negative.")
            .When(x => x.MinPrice.HasValue);

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Maximum price cannot be negative.")
            .When(x => x.MaxPrice.HasValue);

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(x => x.MinPrice!.Value)
            .WithMessage("Maximum price must be greater than or equal to minimum price.")
            .When(x => x.MinPrice.HasValue && x.MaxPrice.HasValue);

        RuleFor(x => x.SortBy)
            .Must(x => AllowedSortColumns.Contains(x!.Trim().ToLowerInvariant()))
            .WithMessage("SortBy can only be 'price', 'name', or 'createdAt'.")
            .When(x => !string.IsNullOrWhiteSpace(x.SortBy));

        RuleFor(x => x.SortOrder)
            .Must(x => AllowedSortOrders.Contains(x!.Trim().ToLowerInvariant()))
            .WithMessage("SortOrder can only be 'asc' or 'desc'.")
            .When(x => !string.IsNullOrWhiteSpace(x.SortOrder));
    }
}
