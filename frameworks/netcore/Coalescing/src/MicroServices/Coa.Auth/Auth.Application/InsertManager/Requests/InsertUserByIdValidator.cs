using FluentValidation;

namespace Auth.Application.InsertManager.Requests;

public sealed class InsertUserByIdValidator : AbstractValidator<InsertUserRequest>
{
    public InsertUserByIdValidator()
    {
        RuleFor(expression => expression.UserName)
            .NotEmpty()
            .Length(4, 30).WithMessage("Username must be between 4 and 20 characters");

        RuleFor(expression => expression.FirstName)
            .NotEmpty()
            .Length(2, 20).WithMessage("First name must be between 2 and 20 characters");

        RuleFor(expression => expression.MiddleName)
            .MaximumLength(20).WithMessage("Middle name must not exceed 20 characters");

        RuleFor(expression => expression.LastName)
            .NotEmpty()
            .Length(2, 20).WithMessage("Last name must be between 2 and 20 characters");

        RuleFor(expression => expression.Email)
            .NotEmpty()
            .Length(5, 255).WithMessage("Email must be between 5 and 255 characters")
            .EmailAddress();

        RuleFor(expression => expression.Password)
            .NotEmpty()
            .Length(8, 60).WithMessage("Password must be between 8 and 60 characters")
            .Matches("'(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[^a-zA-Z0-9]).{8,}'").WithMessage(
                "Password must contain at least 8 characters, one uppercase, one lowercase, one number and one special character");
    }
}