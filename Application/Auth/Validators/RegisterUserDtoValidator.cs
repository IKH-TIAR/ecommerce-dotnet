using Ecommerce.Application.Auth.Dtos;
using FluentValidation;

namespace Ecommerce.Application.Auth.Validators;

public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserDtoValidator()
    {
            RuleFor(x => x.FullName)                                   
            .NotEmpty().WithMessage("Full name is required.")      
            .MaximumLength(150).WithMessage("Full name cannot exceed 150 characters.");                                            
                                                                       
            RuleFor(x => x.Email)                                      
            .NotEmpty().WithMessage("Email is required.")          
            .EmailAddress().WithMessage("A valid email address is required.")                                                          
            .MaximumLength(255).WithMessage("Email cannot exceed 255 characters.");                                                   
                                                                       
            RuleFor(x => x.Password)                                   
            .NotEmpty().WithMessage("Password is required.")       
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
    }
}