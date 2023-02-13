using FluentValidation;

namespace Liup.Authorization.Application.Authorization.Manager.Requests;

public sealed class AuthenticateUserByIdValidator : AbstractValidator<AuthenticateUserRequest>
{
    public AuthenticateUserByIdValidator()
    {
        RuleFor(expression => expression.UserName)
            .NotEmpty()
            .MinimumLength(4).WithMessage("Username must not be less than 4 characters")
            .MaximumLength(20).WithMessage("Username must not exceed 20 characters");

        RuleFor(expression => expression.FirstName)
            .NotEmpty()
            .MinimumLength(2).WithMessage("First name must not be less than 2 characters")
            .MaximumLength(20).WithMessage("First name must not exceed 20 characters");

        RuleFor(expression => expression.MiddleName)
            .MaximumLength(20).WithMessage("Middle name must not exceed 20 characters");

        RuleFor(expression => expression.LastName)
            .NotEmpty()
            .MinimumLength(2).WithMessage("Last name must not be less than 2 characters")
            .MaximumLength(20).WithMessage("Last name must not exceed 20 characters");

        RuleFor(expression => expression.Email)
            .NotEmpty()
            .MinimumLength(5).WithMessage("Email must not be less than 5 characters")
            .MaximumLength(255).WithMessage("Email must not exceed 255 characters")
            .EmailAddress();

        RuleFor(expression => expression.Password)
            .NotEmpty()
            .MinimumLength(8).WithMessage("Password must not be less than 8 characters")
            .MaximumLength(255).WithMessage("Password must not exceed 60 characters")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,60}$").WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one number");
    }
}
