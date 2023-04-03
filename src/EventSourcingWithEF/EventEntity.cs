namespace EventSourcingWithEF;

public abstract class EventEntity<TId, TEvent>
{
    public TId Id { get; set; } = default!;
    public int Version { get; set; }
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "Entity")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Entity")]
    public List<TEvent> Events { get; set; } = new();
}

public class NameEvent
{
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
    public required string NewName { get; init; }
}
