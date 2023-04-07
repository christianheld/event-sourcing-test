using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace EventSourcing.EntitiyFrameworkCore;

/// <summary>
/// Base class to store event streams in PostgreSQL using EFCore.
/// </summary>
/// <remarks>
/// <typeparamref name="TEvent"/> must be (polymorphic) serializable as <c>jsonb</c> using
/// <c>System.Text.Json</c>
/// </remarks>
/// <typeparam name="TEvent">The base type of the events</typeparam>
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
