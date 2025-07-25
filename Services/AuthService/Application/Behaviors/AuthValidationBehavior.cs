using BuildingBlocks.Application.Behaviors;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace AuthService.Application.Behaviors;

/// <summary>
/// Auth-specific validation behavior that integrates with FluentValidation
/// </summary>
/// <typeparam name="TRequest">The request type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public class AuthValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<AuthValidationBehavior<TRequest, TResponse>> _logger;

    public AuthValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators,
        ILogger<AuthValidationBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    public async Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            var requestName = typeof(TRequest).Name;
            var errorMessages = failures.Select(f => f.ErrorMessage);
            
            _logger.LogWarning("Validation failed for {RequestName}: {Errors}", 
                requestName, string.Join(", ", errorMessages));

            throw new ValidationException($"Validation failed for {requestName}", failures);
        }

        return await next();
    }
} 