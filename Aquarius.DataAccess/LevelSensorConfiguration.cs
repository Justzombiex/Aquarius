using Aquarius.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aquarius.Data.Configurations
{
    public class SensorLevelConfiguration : IEntityTypeConfiguration<LevelSensor>
    {
        public void Configure(EntityTypeBuilder<LevelSensor> builder)
        {
            // Nombre de la tabla en la base de datos
            builder.ToTable("SensorLevels");

            // Configuración de la clave primaria
            builder.HasKey(s => s.Id);

            // Relación uno a uno con la entidad Pond
            builder.HasOne(s => s.Pond)
                   .WithOne(p => p.LevelSensor) // Cambiado a una relación uno a uno
                   .HasForeignKey<LevelSensor>(s => s.PondId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Configuración de la propiedad FullPond
            builder.Property(s => s.FullPond)
                   .IsRequired(); // Asegura que FullPond sea obligatorio
        }
    }
}
