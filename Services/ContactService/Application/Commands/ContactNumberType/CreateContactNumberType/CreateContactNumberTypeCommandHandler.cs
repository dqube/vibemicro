using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Repository;
using ContactService.Application.DTOs.ContactNumberType;
using ContactService.Application.Mapping;
using ContactService.Domain.Aggregates.ContactNumberType;
using ContactService.Domain.Repositories;
using ContactService.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace ContactService.Application.Commands.ContactNumberType.CreateContactNumberType;

public sealed class CreateContactNumberTypeCommandHandler : ICommandHandler<CreateContactNumberTypeCommand, ContactNumberTypeDto>
{
    private readonly IContactNumberTypeRepository _contactNumberTypeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateContactNumberTypeCommandHandler> _logger;

    public CreateContactNumberTypeCommandHandler(
        IContactNumberTypeRepository contactNumberTypeRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateContactNumberTypeCommandHandler> logger)
    {
        _contactNumberTypeRepository = contactNumberTypeRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ContactNumberTypeDto> HandleAsync(CreateContactNumberTypeCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating contact number type with name: {Name}", command.Name);

        // Create domain objects
        var typeName = new ContactNumberTypeName(command.Name);

        // Check if type already exists
        var existingType = await _contactNumberTypeRepository.GetByNameAsync(typeName, cancellationToken);
        if (existingType != null)
        {
            throw new InvalidOperationException($"Contact number type with name '{command.Name}' already exists");
        }

        // Create new contact number type
        var contactNumberType = Domain.Aggregates.ContactNumberType.ContactNumberType.Create(typeName, command.Description);

        // Save to repository
        await _contactNumberTypeRepository.AddAsync(contactNumberType, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Contact number type created successfully with ID: {Id}", contactNumberType.Id);

        // Convert to DTO using extension method
        return contactNumberType.ToDto();
    }
} 