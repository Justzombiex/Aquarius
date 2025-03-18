using Aquarius.Data.Configurations;
using Aquarius.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Aquarius.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Farm> Farms { get; set; }
        public DbSet<Pond> Ponds { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<Reading> Readings { get; set; }
        public DbSet<Alert> Alerts { get; set; }

        /// <summary>
        /// Requerired by EntityFrameworkCore for migration.
        /// </summary>
        protected ApplicationContext()
        {
        }

        public ApplicationContext(string connectionString) 
            : base(GetOptions(connectionString))
        {
        }

        
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

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

        private static DbContextOptions GetOptions(string connectionString)
        {
            return NpgsqlDbContextOptionsBuilderExtensions.UseNpgsql(new DbContextOptionsBuilder(), connectionString).Options;
        }

        public class ApplicationContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
        {
            public ApplicationContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();

                try
                {
                    var connectionString = "Host=localhost;Database=AquariusDB;Username=postgres;Password=1234";
                    optionsBuilder.UseNpgsql(connectionString);
                }
                catch (Exception)
                {
                    //handle error here.. means DLL has no sattelite configuration file.
                    throw;
                }

                return new ApplicationContext(optionsBuilder.Options);
            }
        }
    }
}