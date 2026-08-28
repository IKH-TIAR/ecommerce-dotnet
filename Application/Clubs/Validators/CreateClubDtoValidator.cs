using Ecommerce.Application.Clubs.Dtos;
using FluentValidation;

namespace Ecommerce.Application.Clubs.Validators;


public class CreateClubDtoValidator : AbstractValidator<CreateClubDto>
{
    public CreateClubDtoValidator()
    {
        RuleFor(x => x.Name)
        .NotEmpty().WithMessage("Club Name Is Required")
        .MaximumLength(150).WithMessage("Club Name Can Not be More than 150 Characters");

        RuleFor(x => x.Country)
        .NotEmpty().WithMessage("Country Name Is Required")
        .MaximumLength(150).WithMessage("Country Name Can Not be More than 150 Characters");

        RuleFor(x => x.League)
        .NotEmpty().WithMessage("League Name Is Required")
        .MaximumLength(150).WithMessage("League Name Can Not be More than 150 Characters");
    }
}