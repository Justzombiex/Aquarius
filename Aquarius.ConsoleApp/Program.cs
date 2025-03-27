using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aquarius.Data.Repositories;
using Aquarius.Data;
using Aquarius.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Aquarius.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Configurar el servicio de inyección de dependencias
            var serviceProvider = new ServiceCollection()
                .AddDbContext<AquariusDbContext>(options =>
                    options.UseNpgsql("Host=localhost;Database=AquariusDB;Username=postgres;Password=1234"))
                .AddScoped<IFarmRepository, FarmRepository>()
                .AddScoped<IPondRepository, PondRepository>()
                .AddScoped<ITemperatureSensorRepository, TemperatureSensorRepository>()
                .AddScoped<IReadingRepository, ReadingRepository>()
                .AddScoped<IAlertRepository, AlertRepository>()
                .BuildServiceProvider();

            // Obtener el DbContext
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AquariusDbContext>();

                // Crear la base de datos si no existe
                await context.Database.EnsureCreatedAsync();

                // Obtener los repositorios
                var farmRepository = scope.ServiceProvider.GetRequiredService<IFarmRepository>();
                var pondRepository = scope.ServiceProvider.GetRequiredService<IPondRepository>();
                var sensorRepository = scope.ServiceProvider.GetRequiredService<ITemperatureSensorRepository>();
                var readingRepository = scope.ServiceProvider.GetRequiredService<IReadingRepository>();
                var alertRepository = scope.ServiceProvider.GetRequiredService<IAlertRepository>();

                // Crear datos iniciales si no existen
                if (!(await farmRepository.GetAllAsync()).Any())
                {
                    Console.WriteLine("Creando datos de ejemplo...");

                    // Crear granja, estanque y sensor
                    var farm = new Farm { Id = Guid.NewGuid(), Name = "Granja Principal", Location = "Ubicación A" };
                    await farmRepository.AddAsync(farm);

                    var pond = new Pond { Id = Guid.NewGuid(), Name = "Estanque 1", Capacity = 1000, FarmId = farm.Id };
                    await pondRepository.AddAsync(pond);


                    // Crear sensores
                    var temperatureSensor = new TemperatureSensor

                    {
                        Id = Guid.NewGuid(),
                        PondId = pond.Id
                    };

                    var tempSensor = new TemperatureSensor
                    {
                        Id = Guid.NewGuid(),
                        PondId = pond.Id
                    };
                    await sensorRepository.AddAsync(temperatureSensor);
                    await sensorRepository.AddAsync(tempSensor);


                   

                    await context.SaveChangesAsync(); // Asegura que todo se guarda correctamente
                    Console.WriteLine("Lecturas de temperatura y alertas creadas correctamente.");
                }

                // Mostrar información de la base de datos
                Console.WriteLine("\nInformación de la base de datos:");
                var farms = await farmRepository.GetAllAsync();
                foreach (var farm in farms)
                {
                    Console.WriteLine($"\nGranja: {farm.Name} ({farm.Location})");
                    var ponds = await pondRepository.GetAllAsync();
                    foreach (var pondr in ponds.Where(p => p.FarmId == farm.Id))
                    {
                        Console.WriteLine($"  Estanque: {pondr.Name} (Capacidad: {pondr.Capacity})");
                        var sensors = await sensorRepository.GetAllAsync();
                        foreach (var sensor in sensors.Where(s => s.PondId == pondr.Id))
                        {

                            var readings = await readingRepository.GetAllAsync();
                            foreach (var reading in readings.Where(r => r.SensorId == sensor.Id))
                            {
                                Console.WriteLine($"      Lectura: {reading.Value} ({reading.Timestamp})");
                            }
                        }
                    }
                }

                // Mostrar alertas generadas
                Console.WriteLine("\nAlertas generadas:");
                var alerts = await alertRepository.GetAllAsync();
                foreach (var alert in alerts)
                {
                    Console.WriteLine($"  Alerta: {alert.Message} ({alert.TimeStamp}) - Estanque: {alert.Pond.Name}");
                }

                Console.WriteLine("\nPresiona cualquier tecla para salir...");
                Console.ReadKey();
            }
        }
    }
}
