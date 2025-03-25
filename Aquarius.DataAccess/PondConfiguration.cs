using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Aquarius.Domain;

namespace Aquarius.Data.Configurations
{
    public class PondConfiguration : IEntityTypeConfiguration<Pond>
    {
        public void Configure(EntityTypeBuilder<Pond> builder)
        {
            // Nombre de la tabla en la base de datos
            builder.ToTable("Ponds");

            // Configurar la clave primaria
            builder.HasKey(p => p.Id);

            // Configurar la propiedad Name
            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            // Configurar la propiedad Capacity
            builder.Property(p => p.Capacity)
                   .IsRequired();

            // Relación muchos a uno con Farm
            builder.HasOne(p => p.Farm)
                   .WithMany(f => f.Ponds)
                   .HasForeignKey(p => p.FarmId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relación uno a uno con TemperatureSensor
            builder.HasOne(p => p.TemperatureSensor)
                   .WithOne(s => s.Pond)
                   .HasForeignKey<TemperatureSensor>(s => s.PondId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relación uno a uno con LevelSensor
            builder.HasOne(p => p.LevelSensor)
                   .WithOne(s => s.Pond)
                   .HasForeignKey<LevelSensor>(s => s.PondId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
