using Ecommerce.Application.Auth.Dtos;
using FluentValidation;

namespace Ecommerce.Application.Auth.Validators;


public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email)
        .NotEmpty().WithMessage("Email is required")
        .EmailAddress().WithMessage("A Valid Email Adress is Required");

        RuleFor(x => x.Password)
        .NotEmpty().WithMessage("Password is Required");
    }
}