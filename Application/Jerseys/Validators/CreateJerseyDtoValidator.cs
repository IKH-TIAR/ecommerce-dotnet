using Ecommerce.Application.Jerseys.Dtos;
using FluentValidation;

namespace Ecommerce.Application.Jerseys.Validators;

public class CreateJerseyDtoValidator : AbstractValidator<CreateJerseyDto>
{
    public CreateJerseyDtoValidator()
    {
        RuleFor(x => x.Name)
        .NotEmpty().WithMessage("Jersey Nmae is Required")
        .MaximumLength(150).WithMessage("Jersey name cannot exceed 150 characters");

        RuleFor(x => x.Club)
        .NotEmpty().WithMessage("Club Nmae is Required")
        .MaximumLength(100).WithMessage("Club name cannot exceed 100 characters");

        RuleFor(x => x.Price)
        .GreaterThan(0).WithMessage("Price Must be greater than 0 BDT");

        RuleFor(x => x.StockQuantity)
        .GreaterThanOrEqualTo(0).WithMessage("Stock Quantity Cannot be negative");

        RuleFor(x => x.ImageUrls)
        .NotEmpty().WithMessage("At Least One Image Is Required");
    }
}