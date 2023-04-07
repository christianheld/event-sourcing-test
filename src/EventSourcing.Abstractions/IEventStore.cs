namespace EventSourcing;

/// <summary>
/// Interface to load and perist events.
/// </summary>
/// <typeparam name="TEvent">The event</typeparam>
public interface IEventStore<TEvent> where TEvent : Event
{
    /// <summary>
    /// Load all events of an event stream.
    /// </summary>
    /// <param name="id">The event stream Id.</param>
    /// <returns>The list of events.</returns>
    Task<IReadOnlyList<TEvent>?> LoadEventsAsync(Guid id);

    /// <summary>
    /// Appends an event to the event store.
    /// </summary>
    /// <param name="id">The event stream id.</param>
    /// <param name="events">The event.</param>
    /// <param name="version">The current version.</param>
    /// <returns>The new version.</returns>
    Task AppendAsync(Guid id, IEnumerable<TEvent> events, int version);
}
