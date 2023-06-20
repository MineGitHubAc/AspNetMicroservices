using FluentValidation;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder
{
    public class CheckoutOrderCommandValidator:AbstractValidator<CheckoutOrderCommand>
    {
        public CheckoutOrderCommandValidator()
        {
            RuleFor(s=>s.UserName)
                .NotEmpty().WithMessage("{UserName} is required")
                .NotNull()
                .MaximumLength(50).WithMessage("{UserName} must not exceed 50 characters");

            RuleFor(s => s.EmailAddress)
                .NotEmpty().WithMessage("{EmailAddress} is required");

            RuleFor(s => s.TotalPrice)
            .NotEmpty().WithMessage("{TotalPrice} is required").GreaterThan(-1).WithMessage("{TotalPrice} should be greater than -1");
        }
    }
}
