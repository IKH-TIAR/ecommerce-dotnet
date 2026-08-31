using Ecommerce.Application.Orders.Dtos;
using FluentValidation;

namespace Ecommerce.Application.Orders.Validators;

public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderDtoValidator()
    {
        RuleFor(x => x.ShippingAddress)
            .NotEmpty().WithMessage("Shipping address is required.")
            .MaximumLength(500).WithMessage("Shipping address cannot exceed 500 characters.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^(?:\+8801|8801|01)[3-9]\d{8}$")
            .WithMessage("Please provide a valid Bangladeshi phone number (e.g. 017XXXXXXXX or +88017XXXXXXXX).");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one item must be included in the order.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.JerseyId)
                .NotEmpty().WithMessage("A valid JerseyId is required for every item.");

            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("Item quantity must be at least 1.")
                .LessThanOrEqualTo(100).WithMessage("Cannot order more than 100 units of a single item.");
        });
    }
}
