using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Repository;
using ContactService.Application.DTOs.ContactNumberType;
using ContactService.Domain.Repositories;
using ContactService.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace ContactService.Application.Commands.ContactNumberType.UpdateContactNumberType;

public sealed class UpdateContactNumberTypeCommandHandler : ICommandHandler<UpdateContactNumberTypeCommand, ContactNumberTypeDto>
{
    private readonly IContactNumberTypeRepository _contactNumberTypeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateContactNumberTypeCommandHandler> _logger;

    public UpdateContactNumberTypeCommandHandler(
        IContactNumberTypeRepository contactNumberTypeRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateContactNumberTypeCommandHandler> logger)
    {
        _contactNumberTypeRepository = contactNumberTypeRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ContactNumberTypeDto> HandleAsync(UpdateContactNumberTypeCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating contact number type with ID: {Id}", command.Id);

        // Get existing entity
        var contactNumberType = await _contactNumberTypeRepository.GetByIdAsync(command.Id, cancellationToken);
        if (contactNumberType == null)
        {
            throw new InvalidOperationException($"Contact number type with ID '{command.Id}' not found");
        }

        // Create domain objects
        var typeName = new ContactNumberTypeName(command.Name);

        // Check if new name conflicts with existing type (if name changed)
        if (!contactNumberType.IsNamedAs(typeName))
        {
            var existingTypeWithName = await _contactNumberTypeRepository.GetByNameAsync(typeName, cancellationToken);
            if (existingTypeWithName != null)
            {
                throw new InvalidOperationException($"Contact number type with name '{command.Name}' already exists");
            }
        }

        // Update entity
        contactNumberType.Update(typeName, command.Description);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Contact number type updated successfully with ID: {Id}", contactNumberType.Id);

        // Manual mapping to DTO
        return new ContactNumberTypeDto
        {
            Id = contactNumberType.Id.Value,
            Name = contactNumberType.Name.Value,
            Description = contactNumberType.Description,
            CreatedAt = contactNumberType.CreatedAt,
            CreatedBy = contactNumberType.CreatedBy,
            UpdatedAt = contactNumberType.UpdatedAt,
            UpdatedBy = contactNumberType.UpdatedBy
        };
    }
} 