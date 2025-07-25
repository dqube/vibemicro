namespace ContactService.Application.DTOs.Common;

public sealed record PhoneNumberDto
{
    public string Value { get; init; } = string.Empty;
    public string DigitsOnly { get; init; } = string.Empty;
    public bool IsInternational { get; init; }
    public string? CountryCode { get; init; }
} 