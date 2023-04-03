using Microsoft.EntityFrameworkCore;

namespace EventSourcingWithEF;

public class PersonEntity : EventEntity<Guid, NameEvent>
{
}

public interface IPersonStore : IEventStore<Guid, NameEvent>
{
}

public class PersonStore : EventStore<PersonEntity, Guid, NameEvent>, IPersonStore
{
    public PersonStore(DbSet<PersonEntity> dbset) : base(dbset)
    {
    }
}

public class Person
{
    private readonly List<NameEvent> _unconfirmedEvents = new();
    private string _name = "";
    private int _confirmedVersion;

    public Person(Guid id)
        : this(id, Array.Empty<NameEvent>())
    {
    }

    public Person(Guid id, IReadOnlyList<NameEvent> events)
    {
        ArgumentNullException.ThrowIfNull(events);

        Id = id;
        foreach (var e in events)
        {
            Apply(e);
        }

        _confirmedVersion = Version;
    }

    public string Name
    {
        get => _name;
        set
        {
            var e = new NameEvent { NewName = value };
            _unconfirmedEvents.Add(e);
            Apply(e);
        }
    }

    public int Version { get; private set; }
    public Guid Id { get; }

    public async Task ConfirmAsync(IPersonStore store)
    {
        ArgumentNullException.ThrowIfNull(store);

        await store.AppendAsync(Id, _unconfirmedEvents, _confirmedVersion).ConfigureAwait(false);

        _confirmedVersion = Version;
        _unconfirmedEvents.Clear();
    }

    private void Apply(NameEvent @event)
    {
        _name = @event.NewName;
        Version++;
    }
}
