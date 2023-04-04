namespace EventSourcingWithEF;

public class NameEvent : PersonEvent
{
    public required string NewName { get; init; }
}
