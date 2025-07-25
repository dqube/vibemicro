using BuildingBlocks.Application.CQRS.Commands;
using BuildingBlocks.Domain.Repository;
using ContactService.Application.DTOs.AddressType;
using ContactService.Domain.Repositories;
using ContactService.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace ContactService.Application.Commands.AddressType.CreateAddressType;

public sealed class CreateAddressTypeCommandHandler : ICommandHandler<CreateAddressTypeCommand, AddressTypeDto>
{
    private readonly IAddressTypeRepository _addressTypeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateAddressTypeCommandHandler> _logger;

    public CreateAddressTypeCommandHandler(
        IAddressTypeRepository addressTypeRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateAddressTypeCommandHandler> logger)
    {
        _addressTypeRepository = addressTypeRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<AddressTypeDto> HandleAsync(CreateAddressTypeCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating address type with name: {Name}", command.Name);

        // Create domain objects
        var typeName = new AddressTypeName(command.Name);

        // Check if type already exists
        var existingType = await _addressTypeRepository.GetByNameAsync(typeName, cancellationToken);
        if (existingType != null)
        {
            throw new InvalidOperationException($"Address type with name '{command.Name}' already exists");
        }

        // Create new address type
        var addressType = Domain.Aggregates.AddressType.AddressType.Create(typeName, command.Description);

        // Save to repository
        await _addressTypeRepository.AddAsync(addressType, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Address type created successfully with ID: {Id}", addressType.Id);

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