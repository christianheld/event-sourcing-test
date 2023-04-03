using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EventSourcingWithEF;

public class EventStore<T, TId, TEvent> : IEventStore<TId, TEvent>
    where T : EventEntity<TId, TEvent>, new()
{
    private readonly DbSet<T> _dbset;

    public EventStore(DbSet<T> dbset)
    {
        _dbset = dbset;
    }

    public async Task AppendAsync(TId id, IEnumerable<TEvent> events, int version)
    {
        var entity = await _dbset.FindAsync(id)
           .ConfigureAwait(false);

        if (entity is null)
        {
            if (version != 0)
            {
                throw new InvalidOperationException("Version must be 0 for new streams.");
            }

            entity = new T
            {
                Id = id,
                Events = events.ToList(),
            };

            _dbset.Add(entity);
        }
        else
        {
            if (entity.Events.Count != version)
            {
                throw new InvalidOperationException(
                    $"Version mismatch, expected: {version} but was {entity.Events.Count}");
            }

            entity.Events.AddRange(events);
            entity.Version = entity.Events.Count;
        }
    }

    public async Task<IReadOnlyList<TEvent>?> LoadEventsAsync(TId id)
    {
        var entity = await _dbset.FindAsync(id)
            .ConfigureAwait(false);

        if (entity is null)
        {
            return null;
        }

        return entity.Events;
    }
}
