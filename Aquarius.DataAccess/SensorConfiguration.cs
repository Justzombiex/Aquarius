using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Aquarius.Domain;

namespace Aquarius.Data.Configurations
{
    public class SensorConfiguration : IEntityTypeConfiguration<Sensor>
    {
        public void Configure(EntityTypeBuilder<Sensor> builder)
        {

            builder.HasKey(s => s.Id);

            builder.Property(s => s.VariableType)
                   .IsRequired();

            // Relación muchos a uno con Pond
            builder.HasOne(s => s.Pond)
                   .WithMany(p => p.Sensors)
                   .HasForeignKey(s => s.PondId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relación uno a muchos con Readings
            builder.HasMany(s => s.Readings)
                   .WithOne(r => r.Sensor)
                   .HasForeignKey(r => r.SensorId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}