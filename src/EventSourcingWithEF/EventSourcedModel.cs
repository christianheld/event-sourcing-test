using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace EventSourcingWithEF;

/// <summary>
/// Base class for a event sourced model
/// </summary>
/// <remarks>
/// Base class for models with an individual event stream.
/// </remarks>
/// <typeparam name="TEvent">The type of the event</typeparam>
public abstract class EventSourcedModel<TEvent> where TEvent : Event
{
    //private ImmutableList<TEvent> _confirmedEvents = ImmutableList<TEvent>.Empty;
    private List<TEvent> _unconfirmedEvents = new();

    protected EventSourcedModel(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }

    /// <summary>
    /// Gets the current version of the event model
    /// </summary>
    public int Version { get; private set; }

    /// <summary>
    /// The confirmed (last saved) version.
    /// </summary>
    public int ConfirmedVersion { get; private set; }

    /// <summary>
    /// Loads the state from a existing event stream.
    /// </summary>
    /// <param name="events">The event stream.</param>
    public void Load(IEnumerable<TEvent> events)
    {
        ArgumentNullException.ThrowIfNull(events);

        foreach (var @event in events)
        {
            Apply(@event);
            Version++;
        }

        ConfirmedVersion = Version;
    }

    /// <summary>
    /// Stores all events and updates <see cref="ConfirmedVersion"/>.
    /// </summary>
    /// <param name="store">The event store.</param>
    public async Task ConfirmAsync(IEventStore<TEvent> store)
    {
        ArgumentNullException.ThrowIfNull(store);

        await store.AppendAsync(Id, _unconfirmedEvents, ConfirmedVersion)
            .ConfigureAwait(false);

        _unconfirmedEvents.Clear();
        ConfirmedVersion = Version;
    }

    /// <summary>
    /// Raises an event and updates the model.
    /// </summary>
    /// <remarks>
    /// This will store the event in the model and increase the version number. Call
    /// <see cref="ConfirmAsync(IEventStore{TEvent})"/> to perist changes in the event store.
    /// </remarks>
    /// <seealso cref="Apply(TEvent)"/>
    /// <seealso cref="IEventStore{TEvent}"/>
    /// <param name="event">The event.</param>
    [SuppressMessage(
        "Design",
        "CA1030:Use events where appropriate",
        Justification = "We are not talking traditional .NET events here")]
    public void RaiseEvent(TEvent @event)
    {
        Apply(@event);
        _unconfirmedEvents.Add(@event);
        Version++;
    }

    /// <summary>
    /// Applies an event and updates state of the model.
    /// </summary>
    /// <remarks>
    /// This method should never be called by inheritors.
    /// </remarks>
    /// <param name="event">The event</param>
    protected abstract void Apply(TEvent @event);
}
