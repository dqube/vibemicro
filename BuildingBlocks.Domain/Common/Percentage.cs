using BuildingBlocks.Domain.Guards;
using BuildingBlocks.Domain.ValueObjects;

namespace BuildingBlocks.Domain.Common;

/// <summary>
/// Value object representing a percentage
/// </summary>
public sealed record Percentage : SingleValueObject<decimal>
{
    /// <summary>
    /// Gets the percentage as a decimal value (0.0 to 1.0)
    /// </summary>
    public decimal AsDecimal => Value / 100m;

    /// <summary>
    /// Gets the percentage as a fraction (0 to 1)
    /// </summary>
    public double AsDouble => (double)(Value / 100m);

    /// <summary>
    /// Initializes a new instance of the Percentage class
    /// </summary>
    /// <param name="value">The percentage value (0 to 100)</param>
    public Percentage(decimal value) : base(ValidateAndNormalize(value))
    {
    }

    /// <summary>
    /// Creates a percentage from a decimal value (0.0 to 1.0)
    /// </summary>
    /// <param name="decimalValue">The decimal value</param>
    /// <returns>A percentage instance</returns>
    public static Percentage FromDecimal(decimal decimalValue)
    {
        Guard.InRange(decimalValue, 0m, 1m);
        return new Percentage(decimalValue * 100m);
    }

    /// <summary>
    /// Creates a percentage from a fraction
    /// </summary>
    /// <param name="numerator">The numerator</param>
    /// <param name="denominator">The denominator</param>
    /// <returns>A percentage instance</returns>
    public static Percentage FromFraction(decimal numerator, decimal denominator)
    {
        Guard.Positive(denominator);
        return FromDecimal(numerator / denominator);
    }

    /// <summary>
    /// Gets the zero percentage (0%)
    /// </summary>
    public static Percentage Zero => new(0);

    /// <summary>
    /// Gets the full percentage (100%)
    /// </summary>
    public static Percentage Full => new(100);

    /// <summary>
    /// Applies this percentage to a value
    /// </summary>
    /// <param name="value">The value to apply the percentage to</param>
    /// <returns>The calculated result</returns>
    public decimal Of(decimal value)
    {
        return value * AsDecimal;
    }

    /// <summary>
    /// Adds two percentages
    /// </summary>
    /// <param name="left">The first percentage</param>
    /// <param name="right">The second percentage</param>
    /// <returns>The sum of the percentages</returns>
    public static Percentage operator +(Percentage left, Percentage right)
    {
        return new Percentage(left.Value + right.Value);
    }

    /// <summary>
    /// Subtracts two percentages
    /// </summary>
    /// <param name="left">The first percentage</param>
    /// <param name="right">The second percentage</param>
    /// <returns>The difference of the percentages</returns>
    public static Percentage operator -(Percentage left, Percentage right)
    {
        return new Percentage(left.Value - right.Value);
    }

    /// <summary>
    /// Multiplies a percentage by a scalar
    /// </summary>
    /// <param name="percentage">The percentage</param>
    /// <param name="multiplier">The multiplier</param>
    /// <returns>The multiplied percentage</returns>
    public static Percentage operator *(Percentage percentage, decimal multiplier)
    {
        return new Percentage(percentage.Value * multiplier);
    }

    /// <summary>
    /// Validates and normalizes the percentage value
    /// </summary>
    /// <param name="value">The value to validate</param>
    /// <returns>The validated value</returns>
    private static decimal ValidateAndNormalize(decimal value)
    {
        if (value < 0 || value > 100)
            throw new ArgumentOutOfRangeException(nameof(value), value, "Percentage must be between 0 and 100");

        return Math.Round(value, 4); // Round to 4 decimal places for precision
    }

    /// <summary>
    /// Returns the string representation of the percentage
    /// </summary>
    public override string ToString()
    {
        return $"{Value:F2}%";
    }
} 