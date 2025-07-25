using BuildingBlocks.Domain.ValueObjects;

namespace BuildingBlocks.Domain.Common;

/// <summary>
/// Value object representing a date range
/// </summary>
public record DateRange : ValueObject
{
    /// <summary>
    /// Gets the start date of the range
    /// </summary>
    public DateTime StartDate { get; private set; }

    /// <summary>
    /// Gets the end date of the range
    /// </summary>
    public DateTime EndDate { get; private set; }

    /// <summary>
    /// Initializes a new instance of the DateRange class
    /// </summary>
    /// <param name="startDate">The start date</param>
    /// <param name="endDate">The end date</param>
    public DateRange(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
            throw new ArgumentException("Start date cannot be greater than end date");

        StartDate = startDate;
        EndDate = endDate;
    }

    /// <summary>
    /// Gets the atomic values that define the value object's equality
    /// </summary>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return StartDate;
        yield return EndDate;
    }

    /// <summary>
    /// Gets the duration of the date range
    /// </summary>
    public TimeSpan Duration => EndDate - StartDate;

    /// <summary>
    /// Gets the number of days in the date range
    /// </summary>
    public int Days => (int)Duration.TotalDays + 1; // +1 to include both start and end days

    /// <summary>
    /// Checks if the date range contains the specified date
    /// </summary>
    /// <param name="date">The date to check</param>
    /// <returns>True if the date is within the range</returns>
    public bool Contains(DateTime date)
    {
        return date >= StartDate && date <= EndDate;
    }

    /// <summary>
    /// Checks if this date range overlaps with another date range
    /// </summary>
    /// <param name="other">The other date range</param>
    /// <returns>True if the ranges overlap</returns>
    public bool OverlapsWith(DateRange other)
    {
        if (other == null) return false;
        return StartDate <= other.EndDate && EndDate >= other.StartDate;
    }

    /// <summary>
    /// Checks if this date range is fully contained within another date range
    /// </summary>
    /// <param name="other">The other date range</param>
    /// <returns>True if this range is contained within the other range</returns>
    public bool IsContainedIn(DateRange other)
    {
        if (other == null) return false;
        return StartDate >= other.StartDate && EndDate <= other.EndDate;
    }

    /// <summary>
    /// Checks if this date range fully contains another date range
    /// </summary>
    /// <param name="other">The other date range</param>
    /// <returns>True if this range contains the other range</returns>
    public bool Contains(DateRange other)
    {
        if (other == null) return false;
        return other.IsContainedIn(this);
    }

    /// <summary>
    /// Gets the intersection of this date range with another date range
    /// </summary>
    /// <param name="other">The other date range</param>
    /// <returns>The intersection date range, or null if no overlap</returns>
    public DateRange? Intersect(DateRange other)
    {
        if (other == null || !OverlapsWith(other))
            return null;

        var intersectionStart = StartDate > other.StartDate ? StartDate : other.StartDate;
        var intersectionEnd = EndDate < other.EndDate ? EndDate : other.EndDate;

        return new DateRange(intersectionStart, intersectionEnd);
    }

    /// <summary>
    /// Extends the date range by the specified number of days
    /// </summary>
    /// <param name="days">The number of days to extend (can be negative to shrink)</param>
    /// <returns>A new extended date range</returns>
    public DateRange Extend(int days)
    {
        return new DateRange(StartDate.AddDays(-days), EndDate.AddDays(days));
    }

    /// <summary>
    /// Shifts the date range by the specified number of days
    /// </summary>
    /// <param name="days">The number of days to shift</param>
    /// <returns>A new shifted date range</returns>
    public DateRange Shift(int days)
    {
        return new DateRange(StartDate.AddDays(days), EndDate.AddDays(days));
    }

    /// <summary>
    /// Creates a date range for a single day
    /// </summary>
    /// <param name="date">The date</param>
    /// <returns>A new date range for the single day</returns>
    public static DateRange SingleDay(DateTime date)
    {
        return new DateRange(date, date);
    }

    /// <summary>
    /// Creates a date range for the current month
    /// </summary>
    /// <returns>A new date range for the current month</returns>
    public static DateRange CurrentMonth()
    {
        var now = DateTime.Now;
        var start = new DateTime(now.Year, now.Month, 1);
        var end = start.AddMonths(1).AddDays(-1);
        return new DateRange(start, end);
    }

    /// <summary>
    /// Creates a date range for the current year
    /// </summary>
    /// <returns>A new date range for the current year</returns>
    public static DateRange CurrentYear()
    {
        var now = DateTime.Now;
        var start = new DateTime(now.Year, 1, 1);
        var end = new DateTime(now.Year, 12, 31);
        return new DateRange(start, end);
    }

    /// <summary>
    /// Creates a date range from a start date with a specified duration
    /// </summary>
    /// <param name="startDate">The start date</param>
    /// <param name="duration">The duration</param>
    /// <returns>A new date range</returns>
    public static DateRange FromDuration(DateTime startDate, TimeSpan duration)
    {
        return new DateRange(startDate, startDate.Add(duration));
    }

    /// <summary>
    /// Returns the string representation of the date range
    /// </summary>
    public override string ToString()
    {
        return $"{StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd}";
    }

    /// <summary>
    /// Returns the string representation of the date range with custom format
    /// </summary>
    /// <param name="format">The date format</param>
    /// <returns>The formatted date range string</returns>
    public string ToString(string format)
    {
        return $"{StartDate.ToString(format)} to {EndDate.ToString(format)}";
    }
} 