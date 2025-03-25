using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Aquarius.Domain;

namespace Aquarius.Data.Configurations
{
    public class TemperatureSensorConfiguration : IEntityTypeConfiguration<TemperatureSensor>
    {
        public void Configure(EntityTypeBuilder<TemperatureSensor> builder)
        {
            // Nombre de la tabla en la base de datos
            builder.ToTable("TemperatureSensors");

            // Configuración de la clave primaria
            builder.HasKey(s => s.Id);

            // Relación uno a uno con Pond
            builder.HasOne(s => s.Pond)
                   .WithOne(p => p.TemperatureSensor) // Cambiado a una relación uno a uno
                   .HasForeignKey<TemperatureSensor>(s => s.PondId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relación uno a muchos con Readings (lecturas del sensor)
            builder.HasMany(s => s.Readings)
                   .WithOne(r => r.Sensor) // Asegúrate de que la propiedad en Reading sea TemperatureSensor
                   .HasForeignKey(r => r.SensorId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
