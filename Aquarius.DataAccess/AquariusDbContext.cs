using Aquarius.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Aquarius.Data
{
    public class AquariusDbContext : DbContext
    {
        public DbSet<Farm> Farms { get; set; }
        public DbSet<Pond> Ponds { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<Reading> Readings { get; set; }
        public DbSet<Alert> Alerts { get; set; }

        public AquariusDbContext(DbContextOptions<AquariusDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplicar configuraciones Fluent API
            modelBuilder.ApplyConfiguration(new FarmConfiguration());
            modelBuilder.ApplyConfiguration(new PondConfiguration());
            modelBuilder.ApplyConfiguration(new SensorConfiguration());
            modelBuilder.ApplyConfiguration(new ReadingConfiguration());
            modelBuilder.ApplyConfiguration(new AlertConfiguration());
        }
    }
}