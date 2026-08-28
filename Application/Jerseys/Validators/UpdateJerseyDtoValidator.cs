using Ecommerce.Application.Jerseys.Dtos;
using FluentValidation;

namespace Ecommerce.Application.Jerseys.Validators;

public class UpdateJerseyDtoValidator : AbstractValidator<UpdateJerseyDto>
{
    public UpdateJerseyDtoValidator()
    {
        RuleFor(x => x.Name)
        .NotEmpty().WithMessage("Jersey Name Can Not Be Empty When Provided")
        .MaximumLength(150).WithMessage("Jersey Name Can Not Exceed 150 Character")
        .When(x => x.Name is not null);

        RuleFor(x => x.ClubId)                                                                     
        .NotEmpty().WithMessage("ClubId cannot be empty when provided.")                       
        .When(x => x.ClubId.HasValue); 

        RuleFor(x => x.Price)                             
        .GreaterThan(0).WithMessage("Price must be greater than 0 BDT")                                        
        .When(x => x.Price.HasValue);

        RuleFor(x => x.StockQuantity)                             
        .GreaterThanOrEqualTo(0).WithMessage("Stock quantity Can not be negative")                                        
        .When(x => x.StockQuantity.HasValue);

        RuleFor(x => x.ImageUrls)                         
        .NotEmpty().WithMessage("Image list cannot be empty when provided")                                       
        .When(x => x.ImageUrls is not null); 
    }
}