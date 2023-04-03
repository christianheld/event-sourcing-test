namespace EventSourcingWithEF;

public interface IEventStore<TId, TEvent>
{
    /// <summary>
    /// Load all events of an event stream.
    /// </summary>
    /// <param name="id">The event stream Id.</param>
    /// <returns>The list of events.</returns>
    Task<IReadOnlyList<TEvent>?> LoadEventsAsync(TId id);

    /// <summary>
    /// Appends an event to the event store.
    /// </summary>
    /// <param name="id">The event stream id.</param>
    /// <param name="events">The event.</param>
    /// <param name="version">The current version.</param>
    /// <returns>The new version.</returns>
    Task AppendAsync(TId id, IEnumerable<TEvent> events, int version);
}
