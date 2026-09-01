using Ecommerce.Application.Jerseys.Dtos;
using FluentValidation;

namespace Ecommerce.Application.Jerseys.Validators;

public class CreateJerseyDtoValidator : AbstractValidator<CreateJerseyDto>
{
    public CreateJerseyDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Jersey name is required.")
            .MaximumLength(150).WithMessage("Jersey name cannot exceed 150 characters.");

        RuleFor(x => x.ClubId)
            .NotEmpty().WithMessage("A valid ClubId is required.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0 BDT.");

        RuleFor(x => x.ImageUrls)
            .NotEmpty().WithMessage("At least one image URL is required.");

        RuleFor(x => x.Sizes)
            .NotEmpty().WithMessage("At least one size with stock must be provided.");

        RuleForEach(x => x.Sizes).ChildRules(size =>
        {
            size.RuleFor(s => s.Size)
                .IsInEnum().WithMessage("Invalid jersey size (1=S, 2=M, 3=L, 4=XL, 5=XXL).");

            size.RuleFor(s => s.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative.");
        });
    }
}