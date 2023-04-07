using System.Text.Json.Serialization;

namespace EventSourcing.Sample;

[JsonDerivedType(typeof(NameEvent), "name")]
[JsonDerivedType(typeof(NoOpEvent), "no-op")]
public abstract class PersonEvent : Event
{
}
