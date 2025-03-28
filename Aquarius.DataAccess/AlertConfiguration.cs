using Aquarius.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aquarius.Data.Configurations
{
    public class AlertConfiguration : IEntityTypeConfiguration<Alert>
    {
        public void Configure(EntityTypeBuilder<Alert> builder)
        {
            builder.ToTable("Alerts");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Message)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(a => a.TimeStamp)
                   .IsRequired();

            builder.Property(a => a.IsActive)
                   .IsRequired();

            builder.Property(a => a.AlarmType)
                   .IsRequired()
                   .HasConversion(v => v.ToString(),
                   v => (AlarmType)Enum.Parse(typeof(AlarmType), v));


            // Relación muchos a uno con Pond
            builder.HasOne(a => a.Pond)
                   .WithMany()
                   .HasForeignKey(a => a.PondId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}