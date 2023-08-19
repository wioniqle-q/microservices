using FluentValidation;

namespace Auth.Application.TransferManager.Requests;

public sealed class TransferUserByIdValidator : AbstractValidator<TransferUserRequest>
{
    public TransferUserByIdValidator()
    {
        RuleFor(expression => expression.AccessToken)
            .NotEmpty()
            .MinimumLength(10).WithMessage("AccessToken must be at least 10 characters");

        RuleFor(expression => expression.RefreshToken)
            .NotEmpty()
            .MinimumLength(10).WithMessage("RefreshToken must be at least 10 characters");
    }
}