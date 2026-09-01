using Ecommerce.Application.Jerseys.Dtos;
using FluentValidation;

namespace Ecommerce.Application.Jerseys.Validators;

public class UpdateJerseyDtoValidator : AbstractValidator<UpdateJerseyDto>
{
    public UpdateJerseyDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Jersey name cannot be empty when provided.")
            .MaximumLength(150).WithMessage("Jersey name cannot exceed 150 characters.")
            .When(x => x.Name is not null);

        RuleFor(x => x.ClubId)
            .NotEmpty().WithMessage("ClubId cannot be empty when provided.")
            .When(x => x.ClubId.HasValue);

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0 BDT.")
            .When(x => x.Price.HasValue);

        RuleFor(x => x.ImageUrls)
            .NotEmpty().WithMessage("Image list cannot be empty when provided.")
            .When(x => x.ImageUrls is not null);

        RuleForEach(x => x.Sizes).ChildRules(size =>
        {
            size.RuleFor(s => s.Size)
                .IsInEnum().WithMessage("Invalid jersey size (1=S, 2=M, 3=L, 4=XL, 5=XXL).");

            size.RuleFor(s => s.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative.");
        }).When(x => x.Sizes is not null);
    }
}