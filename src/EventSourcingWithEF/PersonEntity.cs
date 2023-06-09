using System.ComponentModel.DataAnnotations.Schema;

using EventSourcing.EntitiyFrameworkCore;

using Microsoft.EntityFrameworkCore;

namespace EventSourcing.Sample;

[Index(nameof(Name))]
public class PersonEntity : EventEntity<PersonEvent>
{
    [Column("name")]
    public string? Name { get; set; }
}

public interface IPersonStore : IEventStore<PersonEvent>
{
}

public class PersonStore : EventStore<PersonEntity, PersonEvent>, IPersonStore
{
    public PersonStore(PersonContext context)
        : base(context, context.Persons)
    {
    }

}

public class Person : EventSourcedModel<PersonEvent>
{
    private string _name = "";

    public Person(Guid id)
        : base(id)

    {
    }

    public string Name
    {
        get => _name;
        set
        {
            var e = new NameEvent { NewName = value };
            RaiseEvent(e);
        }
    }

    public void NoOp()
    {
        var e = new NoOpEvent { };
        RaiseEvent(e);
    }

    protected override void Apply(PersonEvent @event)
    {
        switch (@event)
        {
            case NameEvent ne:
                _name = ne.NewName;
                break;

            default:
                break;
        }
    }
}
