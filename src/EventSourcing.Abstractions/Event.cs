namespace EventSourcing;

public abstract class Event
{
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}
