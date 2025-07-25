using BuildingBlocks.Domain.ValueObjects;
using System.Globalization;

namespace BuildingBlocks.Domain.Common;

/// <summary>
/// Value object representing money with amount and currency
/// </summary>
public class Money : ValueObject
{
    /// <summary>
    /// Gets the amount of money
    /// </summary>
    public decimal Amount { get; private set; }

    /// <summary>
    /// Gets the currency code (ISO 4217)
    /// </summary>
    public string Currency { get; private set; }

    /// <summary>
    /// Initializes a new instance of the Money class
    /// </summary>
    /// <param name="amount">The amount of money</param>
    /// <param name="currency">The currency code</param>
    public Money(decimal amount, string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be null or empty", nameof(currency));

        if (currency.Length != 3)
            throw new ArgumentException("Currency must be a 3-letter ISO 4217 code", nameof(currency));

        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    /// <summary>
    /// Gets the atomic values that define the value object's equality
    /// </summary>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    /// <summary>
    /// Adds two Money values
    /// </summary>
    public static Money operator +(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return new Money(left.Amount + right.Amount, left.Currency);
    }

    /// <summary>
    /// Subtracts two Money values
    /// </summary>
    public static Money operator -(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return new Money(left.Amount - right.Amount, left.Currency);
    }

    /// <summary>
    /// Multiplies Money by a decimal factor
    /// </summary>
    public static Money operator *(Money money, decimal factor)
    {
        return new Money(money.Amount * factor, money.Currency);
    }

    /// <summary>
    /// Multiplies Money by a decimal factor
    /// </summary>
    public static Money operator *(decimal factor, Money money)
    {
        return money * factor;
    }

    /// <summary>
    /// Divides Money by a decimal factor
    /// </summary>
    public static Money operator /(Money money, decimal factor)
    {
        if (factor == 0)
            throw new DivideByZeroException("Cannot divide money by zero");

        return new Money(money.Amount / factor, money.Currency);
    }

    /// <summary>
    /// Checks if left Money is greater than right Money
    /// </summary>
    public static bool operator >(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return left.Amount > right.Amount;
    }

    /// <summary>
    /// Checks if left Money is less than right Money
    /// </summary>
    public static bool operator <(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return left.Amount < right.Amount;
    }

    /// <summary>
    /// Checks if left Money is greater than or equal to right Money
    /// </summary>
    public static bool operator >=(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return left.Amount >= right.Amount;
    }

    /// <summary>
    /// Checks if left Money is less than or equal to right Money
    /// </summary>
    public static bool operator <=(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return left.Amount <= right.Amount;
    }

    /// <summary>
    /// Creates a new Money instance with zero amount
    /// </summary>
    /// <param name="currency">The currency code</param>
    /// <returns>A new Money instance with zero amount</returns>
    public static Money Zero(string currency)
    {
        return new Money(0, currency);
    }

    /// <summary>
    /// Creates a new Money instance from dollars
    /// </summary>
    /// <param name="amount">The amount in dollars</param>
    /// <returns>A new Money instance in USD</returns>
    public static Money FromDollars(decimal amount)
    {
        return new Money(amount, "USD");
    }

    /// <summary>
    /// Creates a new Money instance from euros
    /// </summary>
    /// <param name="amount">The amount in euros</param>
    /// <returns>A new Money instance in EUR</returns>
    public static Money FromEuros(decimal amount)
    {
        return new Money(amount, "EUR");
    }

    /// <summary>
    /// Checks if this Money has the same currency as another Money
    /// </summary>
    /// <param name="other">The other Money to compare</param>
    /// <returns>True if currencies are the same</returns>
    public bool HasSameCurrency(Money other)
    {
        return Currency.Equals(other?.Currency, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if this Money is zero
    /// </summary>
    public bool IsZero => Amount == 0;

    /// <summary>
    /// Checks if this Money is positive
    /// </summary>
    public bool IsPositive => Amount > 0;

    /// <summary>
    /// Checks if this Money is negative
    /// </summary>
    public bool IsNegative => Amount < 0;

    /// <summary>
    /// Returns the absolute value of this Money
    /// </summary>
    public Money Abs()
    {
        return new Money(Math.Abs(Amount), Currency);
    }

    /// <summary>
    /// Returns the negated value of this Money
    /// </summary>
    public Money Negate()
    {
        return new Money(-Amount, Currency);
    }

    /// <summary>
    /// Rounds the money to the specified number of decimal places
    /// </summary>
    /// <param name="decimals">The number of decimal places</param>
    /// <param name="rounding">The rounding method</param>
    /// <returns>A new Money instance with rounded amount</returns>
    public Money Round(int decimals = 2, MidpointRounding rounding = MidpointRounding.ToEven)
    {
        return new Money(Math.Round(Amount, decimals, rounding), Currency);
    }

    /// <summary>
    /// Formats the money as a string
    /// </summary>
    /// <param name="format">The format string</param>
    /// <param name="provider">The format provider</param>
    /// <returns>The formatted money string</returns>
    public string ToString(string? format, IFormatProvider? provider = null)
    {
        provider ??= CultureInfo.CurrentCulture;
        return $"{Amount.ToString(format, provider)} {Currency}";
    }

    /// <summary>
    /// Returns the string representation of the money
    /// </summary>
    public override string ToString()
    {
        return $"{Amount:F2} {Currency}";
    }

    /// <summary>
    /// Ensures that two Money instances have the same currency
    /// </summary>
    private static void EnsureSameCurrency(Money left, Money right)
    {
        if (left == null) throw new ArgumentNullException(nameof(left));
        if (right == null) throw new ArgumentNullException(nameof(right));

        if (!left.HasSameCurrency(right))
            throw new InvalidOperationException($"Cannot perform operation on different currencies: {left.Currency} and {right.Currency}");
    }
} 