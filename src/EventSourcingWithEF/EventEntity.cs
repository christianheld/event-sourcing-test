using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace EventSourcingWithEF;

public abstract class EventEntity<TEvent> where TEvent : Event
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = default!;

    [Column("version")]
    [ConcurrencyCheck]
    public int Version { get; set; }

    [Column("events", TypeName = "jsonb")]
    [SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "Entity")]
    [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Entity")]
    public List<TEvent> Events { get; set; } = new();
}

public abstract class Event
{
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}
