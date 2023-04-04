// See https://aka.ms/new-console-template for more information

using EventSourcingWithEF;

using Microsoft.EntityFrameworkCore;

using Npgsql;

var csb = new NpgsqlConnectionStringBuilder
{
    Host = "localhost",
    Username = "postgres",
    Password = "postgres"
};

var contextOptions = new DbContextOptionsBuilder<PersonContext>()
    .UseNpgsql(csb.ToString())
    .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information)
    .Options;

using (var personContext = new PersonContext(contextOptions))
{
    await personContext.Database.EnsureCreatedAsync()
        .ConfigureAwait(false);
}

using (var db = new PersonContext(contextOptions))
{
    var count = await db.Persons.CountAsync();
    Console.WriteLine(count);

    var store = new PersonStore(db);
    var personEntity = await db.Persons
        .OrderBy(p => p.Id)
        .LastAsync();

    var events = await store.LoadEventsAsync(personEntity.Id);
    var person = new Person(personEntity.Id);
    if (events is not null)
    {
        person.Load(events);
    }

    Console.WriteLine(person.Id);
    Console.WriteLine(person.Name);

    var personStore = new PersonStore(db);
    for (int i = 0; i < 100; i++)
    {
        var id = Guid.NewGuid();
        var personStream = await personStore.LoadEventsAsync(id).ConfigureAwait(false);

        var person = new Person(id);
        if (personStream != null)
        {
            person.Load(personStream);
        }

        for (int j = 0; j < 100; j++)
        {
            person.Name = $"Name{j}";
            person.NoOp();
        }

        await person.ConfirmAsync(personStore).ConfigureAwait(false);
    }

    await db.SaveChangesAsync().ConfigureAwait(false);


    //for (int i = 0; i < 10_000; i++)
    //{
    //    //if (personStream is { })
    //    //{
    //    //    foreach (var item in personStream)
    //    //    {
    //    //        Console.WriteLine($"{item.Timestamp}: Changed Name to {item.NewName}");
    //    //    }

    //    //    Console.WriteLine("====");
    //    //}


    //    Console.WriteLine($"Name: {person.Name}");

    //    person.Name = $"Name {Guid.NewGuid()}";
    //    Console.WriteLine($"New Name: {person.Name}");

    //    person.NoOp();

    //    await person.ConfirmAsync(personStore).ConfigureAwait(false);
    //}

    await db.SaveChangesAsync().ConfigureAwait(false);
}
