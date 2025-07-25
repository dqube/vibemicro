namespace ContactService.Application.DTOs.Common;

public sealed record AddressDto
{
    public string Line1 { get; init; } = string.Empty;
    public string? Line2 { get; init; }
    public string City { get; init; } = string.Empty;
    public string? State { get; init; }
    public string PostalCode { get; init; } = string.Empty;
    public string CountryCode { get; init; } = string.Empty;
    public string FullAddress { get; init; } = string.Empty;
    public string ShortAddress { get; init; } = string.Empty;
    public bool IsComplete { get; init; }
} 