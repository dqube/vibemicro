using FluentValidation;
using ContactService.Domain.Repositories;
using ContactService.Domain.ValueObjects;

namespace ContactService.Application.Commands.ContactNumberType.CreateContactNumberType;

public sealed class CreateContactNumberTypeCommandValidator : AbstractValidator<CreateContactNumberTypeCommand>
{
    private readonly IContactNumberTypeRepository _contactNumberTypeRepository;

    public CreateContactNumberTypeCommandValidator(IContactNumberTypeRepository contactNumberTypeRepository)
    {
        _contactNumberTypeRepository = contactNumberTypeRepository;

        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(1, 50)
            .Matches(@"^[a-zA-Z\s\-\.&/]+$")
            .WithMessage("Name can only contain letters, spaces, hyphens, dots, ampersands, and forward slashes")
            .MustAsync(BeUniqueName)
            .WithMessage("Contact number type name must be unique");

        RuleFor(x => x.Description)
            .MaximumLength(255)
            .When(x => !string.IsNullOrEmpty(x.Description));
    }

    private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    {
        try
        {
            var typeName = new ContactNumberTypeName(name);
            return !await _contactNumberTypeRepository.ExistsByNameAsync(typeName, cancellationToken);
        }
        catch
        {
            return false; // Invalid name format
        }
    }
} 