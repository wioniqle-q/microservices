using FluentValidation;

namespace Auth.Application.ResetPwManager.Requests;

public sealed class ResetPwUserByIdValidator : AbstractValidator<ResetPwUserRequest>
{
    public ResetPwUserByIdValidator()
    {
        RuleFor(expression => expression.UserName)
            .NotEmpty()
            .Length(4, 20).WithMessage("Username must be between 4 and 20 characters");

        RuleFor(expression => expression.NewPassword)
            .NotEmpty()
            .Length(8, 60).WithMessage("New Password must be between 8 and 60 characters")
            .Matches("'(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[^a-zA-Z0-9]).{8,}'").WithMessage(
                "Password must contain at least 8 characters, one uppercase, one lowercase, one number and one special character");
    }
}