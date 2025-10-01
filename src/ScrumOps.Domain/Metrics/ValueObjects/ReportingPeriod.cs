using ScrumOps.Domain.SharedKernel;

namespace ScrumOps.Domain.Metrics.ValueObjects;

/// <summary>
/// Represents a time period for reporting and metrics aggregation.
/// </summary>
public sealed class ReportingPeriod : ValueObject
{
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }
    public ReportingPeriodType Type { get; }

    private ReportingPeriod(DateTime startDate, DateTime endDate, ReportingPeriodType type)
    {
        if (startDate >= endDate)
            throw new ArgumentException("Start date must be before end date");

        StartDate = startDate;
        EndDate = endDate;
        Type = type;
    }

    public static ReportingPeriod Create(DateTime startDate, DateTime endDate)
    {
        var type = DeterminePeriodType(startDate, endDate);
        return new ReportingPeriod(startDate, endDate, type);
    }

    public static ReportingPeriod CurrentSprint(DateTime sprintStart, DateTime sprintEnd)
    {
        return new ReportingPeriod(sprintStart, sprintEnd, ReportingPeriodType.Sprint);
    }

    public static ReportingPeriod LastSprint(DateTime sprintStart, DateTime sprintEnd)
    {
        return new ReportingPeriod(sprintStart, sprintEnd, ReportingPeriodType.Sprint);
    }

    public static ReportingPeriod CurrentWeek()
    {
        var today = DateTime.Today;
        var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
        var endOfWeek = startOfWeek.AddDays(7);
        return new ReportingPeriod(startOfWeek, endOfWeek, ReportingPeriodType.Week);
    }

    public static ReportingPeriod CurrentMonth()
    {
        var today = DateTime.Today;
        var startOfMonth = new DateTime(today.Year, today.Month, 1);
        var endOfMonth = startOfMonth.AddMonths(1);
        return new ReportingPeriod(startOfMonth, endOfMonth, ReportingPeriodType.Month);
    }

    public static ReportingPeriod CurrentQuarter()
    {
        var today = DateTime.Today;
        var quarterStart = new DateTime(today.Year, ((today.Month - 1) / 3) * 3 + 1, 1);
        var quarterEnd = quarterStart.AddMonths(3);
        return new ReportingPeriod(quarterStart, quarterEnd, ReportingPeriodType.Quarter);
    }

    public static ReportingPeriod Last30Days()
    {
        var today = DateTime.Today;
        return new ReportingPeriod(today.AddDays(-30), today, ReportingPeriodType.Rolling30Days);
    }

    public static ReportingPeriod Custom(DateTime startDate, DateTime endDate)
    {
        return new ReportingPeriod(startDate, endDate, ReportingPeriodType.Custom);
    }

    private static ReportingPeriodType DeterminePeriodType(DateTime startDate, DateTime endDate)
    {
        var duration = endDate - startDate;
        
        return duration.TotalDays switch
        {
            <= 7 => ReportingPeriodType.Week,
            <= 14 => ReportingPeriodType.Sprint,
            <= 31 => ReportingPeriodType.Month,
            <= 92 => ReportingPeriodType.Quarter,
            _ => ReportingPeriodType.Custom
        };
    }

    public int DurationInDays => (int)(EndDate - StartDate).TotalDays;
    
    public bool Contains(DateTime date) => date >= StartDate && date < EndDate;
    
    public string GetDisplayName() => Type switch
    {
        ReportingPeriodType.Sprint => $"Sprint ({StartDate:MMM d} - {EndDate:MMM d})",
        ReportingPeriodType.Week => $"Week of {StartDate:MMM d}",
        ReportingPeriodType.Month => StartDate.ToString("MMMM yyyy"),
        ReportingPeriodType.Quarter => $"Q{((StartDate.Month - 1) / 3) + 1} {StartDate.Year}",
        ReportingPeriodType.Rolling30Days => "Last 30 Days",
        ReportingPeriodType.Custom => $"{StartDate:MMM d} - {EndDate:MMM d}",
        _ => $"{StartDate:MMM d} - {EndDate:MMM d}"
    };

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return StartDate;
        yield return EndDate;
        yield return Type;
    }
}

/// <summary>
/// Types of reporting periods supported by the system.
/// </summary>
public enum ReportingPeriodType
{
    Sprint,
    Week,
    Month,
    Quarter,
    Rolling30Days,
    Custom
}