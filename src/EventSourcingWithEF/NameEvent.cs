namespace EventSourcing.Sample;

public class NameEvent : PersonEvent
{
    public required string NewName { get; init; }
}
