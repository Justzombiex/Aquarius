using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Aquarius.Domain;

namespace Aquarius.Data.Configurations
{
    public class FarmConfiguration : IEntityTypeConfiguration<Farm>
    {
        public void Configure(EntityTypeBuilder<Farm> builder)
        {

            builder.HasKey(f => f.Id); // Clave primaria

            builder.Property(f => f.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(f => f.Location)
                   .IsRequired()
                   .HasMaxLength(200);

            // Relación uno a muchos con Ponds
            builder.HasMany(f => f.Ponds)
                   .WithOne(p => p.Farm)
                   .HasForeignKey(p => p.FarmId)
                   .OnDelete(DeleteBehavior.Cascade); // Eliminación en cascada
        }
    }
}