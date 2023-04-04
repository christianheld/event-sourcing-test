using System.Text.Json.Serialization;

namespace EventSourcingWithEF;

[JsonDerivedType(typeof(NameEvent), "name")]
[JsonDerivedType(typeof(NoOpEvent), "no-op")]
public abstract class PersonEvent : Event
{
}
