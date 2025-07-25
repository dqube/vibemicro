using AuthService.Application.Commands.Authentication;
using FluentValidation;

namespace AuthService.Application.Validation;

/// <summary>
/// Validator for register command
/// </summary>
public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required")
            .Length(3, 50)
            .WithMessage("Username must be between 3 and 50 characters")
            .Matches("^[a-zA-Z0-9_-]+$")
            .WithMessage("Username can only contain letters, numbers, underscores, and hyphens")
            .Must(username => char.IsLetterOrDigit(username[0]))
            .WithMessage("Username must start with a letter or number");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email must be a valid email address")
            .MaximumLength(100)
            .WithMessage("Email cannot exceed 100 characters");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long")
            .Must(ContainUppercase)
            .WithMessage("Password must contain at least one uppercase letter")
            .Must(ContainLowercase)
            .WithMessage("Password must contain at least one lowercase letter")
            .Must(ContainDigit)
            .WithMessage("Password must contain at least one digit")
            .Must(ContainSpecialCharacter)
            .WithMessage("Password must contain at least one special character");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage("Password confirmation is required")
            .Equal(x => x.Password)
            .WithMessage("Password confirmation must match the password");
    }

    private static bool ContainUppercase(string password) => password.Any(char.IsUpper);
    private static bool ContainLowercase(string password) => password.Any(char.IsLower);
    private static bool ContainDigit(string password) => password.Any(char.IsDigit);
    private static bool ContainSpecialCharacter(string password) => password.Any(c => "!@#$%^&*(),.?\":{}|<>".Contains(c));
} 