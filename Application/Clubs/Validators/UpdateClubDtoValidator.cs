using Ecommerce.Application.Clubs.Dtos;
using FluentValidation;

namespace Ecommerce.Application.Clubs.Validators;

public class UpdateClubDtoValidator : AbstractValidator<UpdateClubDto>
{
    public UpdateClubDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Club name cannot be empty when provided.")
            .MaximumLength(100).WithMessage("Club name cannot exceed 100 characters.")
            .When(x => x.Name is not null);

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country name cannot be empty when provided.")
            .MaximumLength(100).WithMessage("Country name cannot exceed 100 characters.")
            .When(x => x.Country is not null);

        RuleFor(x => x.League)
            .NotEmpty().WithMessage("League name cannot be empty when provided.")
            .MaximumLength(100).WithMessage("League name cannot exceed 100 characters.")
            .When(x => x.League is not null);

        RuleFor(x => x.LogoUrl)
            .MaximumLength(500).WithMessage("Logo URL cannot exceed 500 characters.")
            .When(x => x.LogoUrl is not null);
    }
}
