using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Aquarius.Domain;

namespace Aquarius.Data.Configurations
{
    public class ReadingConfiguration : IEntityTypeConfiguration<Reading>
    {
        public void Configure(EntityTypeBuilder<Reading> builder)
        {
            builder.ToTable("Readings");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Value)
                   .IsRequired();

            builder.Property(r => r.Timestamp)
                   .IsRequired();

            // Relación muchos a uno con Sensor
            builder.HasOne(r => r.Sensor)
                   .WithMany(s => s.Readings)
                   .HasForeignKey(r => r.SensorId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}