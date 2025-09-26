using ScrumOps.Domain.SharedKernel;

namespace ScrumOps.Domain.EventManagement.ValueObjects;

/// <summary>
/// Value object representing a time-boxed duration for sprint events.
/// </summary>
public class TimeBox : ValueObject
{
    public TimeSpan Duration { get; }
    public DateTime StartTime { get; }
    public DateTime EndTime => StartTime.Add(Duration);

    public TimeBox(DateTime startTime, TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
            throw new ArgumentException("Duration must be positive.", nameof(duration));

        if (duration > TimeSpan.FromHours(24))
            throw new ArgumentException("Duration cannot exceed 24 hours.", nameof(duration));

        StartTime = startTime;
        Duration = duration;
    }

    public static TimeBox Create(DateTime startTime, TimeSpan duration)
        => new(startTime, duration);

    public static TimeBox CreateWithEndTime(DateTime startTime, DateTime endTime)
    {
        if (endTime <= startTime)
            throw new ArgumentException("End time must be after start time.");

        return new TimeBox(startTime, endTime - startTime);
    }

    public bool IsActive(DateTime currentTime)
        => currentTime >= StartTime && currentTime <= EndTime;

    public bool HasStarted(DateTime currentTime)
        => currentTime >= StartTime;

    public bool HasEnded(DateTime currentTime)
        => currentTime > EndTime;

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return StartTime;
        yield return Duration;
    }

    public override string ToString() 
        => $"{StartTime:yyyy-MM-dd HH:mm} - {EndTime:HH:mm} ({Duration.TotalMinutes:F0} minutes)";
}