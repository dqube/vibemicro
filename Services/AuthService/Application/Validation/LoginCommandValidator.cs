using AuthService.Application.Commands.Authentication;
using FluentValidation;

namespace AuthService.Application.Validation;

/// <summary>
/// Validator for login command
/// </summary>
public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.LoginIdentifier)
            .NotEmpty()
            .WithMessage("Username or email is required")
            .MaximumLength(100)
            .WithMessage("Username or email cannot exceed 100 characters");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(1)
            .WithMessage("Password cannot be empty");
    }
} 