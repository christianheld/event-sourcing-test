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
    .Options;

using (var personContext = new PersonContext(contextOptions))
{
    await personContext.Database.EnsureCreatedAsync()
        .ConfigureAwait(false);
}

using (var db = new PersonContext(contextOptions))
{
    var id = new Guid("140cf25b-fc32-4fa0-b4d8-8f9830326d8f");
    var personStore = new PersonStore(db.Persons);
    var personStream = await personStore.LoadEventsAsync(id).ConfigureAwait(false);

    var person = new Person(id, personStream ?? Array.Empty<NameEvent>());
    for (int i = 0; i < 1000; i++)
    {
        //if (personStream is { })
        //{
        //    foreach (var item in personStream)
        //    {
        //        Console.WriteLine($"{item.Timestamp}: Changed Name to {item.NewName}");
        //    }

        //    Console.WriteLine("====");
        //}


        Console.WriteLine($"Name: {person.Name}");

        person.Name = $"Name {Guid.NewGuid()}";
        Console.WriteLine($"New Name: {person.Name}");

        await person.ConfirmAsync(personStore).ConfigureAwait(false);
    }

    await db.SaveChangesAsync().ConfigureAwait(false);
}
