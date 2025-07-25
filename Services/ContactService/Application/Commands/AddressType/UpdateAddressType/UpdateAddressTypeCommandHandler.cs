using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Repository;
using ContactService.Application.DTOs.AddressType;
using ContactService.Domain.Repositories;
using ContactService.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace ContactService.Application.Commands.AddressType.UpdateAddressType;

public sealed class UpdateAddressTypeCommandHandler : ICommandHandler<UpdateAddressTypeCommand, AddressTypeDto>
{
    private readonly IAddressTypeRepository _addressTypeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateAddressTypeCommandHandler> _logger;

    public UpdateAddressTypeCommandHandler(
        IAddressTypeRepository addressTypeRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateAddressTypeCommandHandler> logger)
    {
        _addressTypeRepository = addressTypeRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<AddressTypeDto> HandleAsync(UpdateAddressTypeCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating address type with ID: {Id}", command.Id);

        // Get existing entity
        var addressType = await _addressTypeRepository.GetByIdAsync(command.Id, cancellationToken);
        if (addressType == null)
        {
            throw new InvalidOperationException($"Address type with ID '{command.Id}' not found");
        }

        // Create domain objects
        var typeName = new AddressTypeName(command.Name);

        // Check if new name conflicts with existing type (if name changed)
        if (!addressType.IsNamedAs(typeName))
        {
            var existingTypeWithName = await _addressTypeRepository.GetByNameAsync(typeName, cancellationToken);
            if (existingTypeWithName != null)
            {
                throw new InvalidOperationException($"Address type with name '{command.Name}' already exists");
            }
        }

        // Update entity
        addressType.Update(typeName, command.Description);

        // Save changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Address type updated successfully with ID: {Id}", addressType.Id);

        // Manual mapping to DTO
        return new AddressTypeDto
        {
            Id = addressType.Id.Value,
            Name = addressType.Name.Value,
            Description = addressType.Description,
            CreatedAt = addressType.CreatedAt,
            CreatedBy = addressType.CreatedBy,
            UpdatedAt = addressType.UpdatedAt,
            UpdatedBy = addressType.UpdatedBy
        };
    }
} 