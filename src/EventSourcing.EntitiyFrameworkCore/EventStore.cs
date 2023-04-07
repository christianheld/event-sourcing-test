using Microsoft.EntityFrameworkCore;

namespace EventSourcing.EntitiyFrameworkCore;

/// <summary>
/// Event store implemation backed by EF Core.
/// </summary>
/// <remarks>
/// Make sure that events can be serialized and deserialized by <c>System.Text.Json</c>
/// </remarks>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TEvent"></typeparam>
public class EventStore<TEntity, TEvent> : IEventStore<TEvent>
    where TEntity : EventEntity<TEvent>, new()
    where TEvent : Event
{
    protected DbSet<TEntity> DbSet { get; }
    protected DbContext DbContext { get; }

    public EventStore(DbContext dbContext, DbSet<TEntity> dbset)
    {
        DbContext = dbContext;
        DbSet = dbset;
    }

    public async Task AppendAsync(Guid id, IEnumerable<TEvent> events, int version)
    {
        var entity = await DbSet.FindAsync(id)
           .ConfigureAwait(false);

        if (entity is null)
        {
            CreateNewEntity(id, version, events);
        }
        else
        {
            UpdateEntity(entity, version, events);
        }
    }

    private void UpdateEntity(TEntity entity, int version, IEnumerable<TEvent> events)
    {
        if (entity.Events.Count != version)
        {
            throw new InvalidOperationException(
                $"Version mismatch, expected: {entity.Events.Count} but was {version}");
        }

        entity.Events.AddRange(events);

        // Manually update change tracker to avoid the need to create potentially expensive
        // snapshot creation.
        DbContext.Entry(entity).Property(e => e.Events).IsModified = true;
        entity.Version = entity.Events.Count;
    }

    private TEntity CreateNewEntity(Guid id, int version, IEnumerable<TEvent> events)
    {
        TEntity? entity;
        if (version != 0)
            throw new InvalidOperationException("Version must be 0 for new streams.");

        var eventList = events.ToList();
        entity = new TEntity
        {
            Id = id,
            Events = eventList,
            Version = eventList.Count,
        };

        DbSet.Add(entity);
        return entity;
    }

    public async Task<IReadOnlyList<TEvent>?> LoadEventsAsync(Guid id)
    {
        var entity = await DbSet.FindAsync(id)
            .ConfigureAwait(false);

        if (entity is null)
            return null;

        return entity.Events;
    }
}
