using Microsoft.EntityFrameworkCore;

namespace EventSourcing.Sample;

public class PersonContext : DbContext
{
    public PersonContext(DbContextOptions<PersonContext> options)
        : base(options)
    {
    }

    public DbSet<PersonEntity> Persons => Set<PersonEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        base.OnModelCreating(modelBuilder);

        var persons = modelBuilder.Entity<PersonEntity>();
        persons.ToTable("persons");
    }
}
