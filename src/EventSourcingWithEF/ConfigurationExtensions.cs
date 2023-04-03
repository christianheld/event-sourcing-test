using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventSourcingWithEF;

public static class ConfigurationExtensions
{
    public static void ConfigureEventStore<T, TId, TEvent>(
        this EntityTypeBuilder<T> builder) where T : EventEntity<TId, TEvent>
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.Version)
            .HasColumnName("version")
            .IsConcurrencyToken();

        builder
            .Property(x => x.Events)
            .HasColumnName("events")
            .HasColumnType("jsonb")
            .IsRequired();
    }
}
