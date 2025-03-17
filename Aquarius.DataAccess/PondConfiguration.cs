using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Aquarius.Domain;

namespace Aquarius.Data.Configurations
{
    public class PondConfiguration : IEntityTypeConfiguration<Pond>
    {
        public void Configure(EntityTypeBuilder<Pond> builder)
        {
            builder.ToTable("Ponds");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(p => p.Capacity)
                   .IsRequired();

            // Relación muchos a uno con Farm
            builder.HasOne(p => p.Farm)
                   .WithMany(f => f.Ponds)
                   .HasForeignKey(p => p.FarmId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relación uno a muchos con Sensors
            builder.HasMany(p => p.Sensors)
                   .WithOne(s => s.Pond)
                   .HasForeignKey(s => s.PondId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}