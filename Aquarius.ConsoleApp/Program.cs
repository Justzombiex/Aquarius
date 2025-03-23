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
                .AddScoped<ISensorRepository, SensorRepository>()
                .AddScoped<IReadingRepository, ReadingRepository>()
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
                var sensorRepository = scope.ServiceProvider.GetRequiredService<ISensorRepository>();
                var readingRepository = scope.ServiceProvider.GetRequiredService<IReadingRepository>();

                // Agregar datos de ejemplo si la base de datos está vacía
                if (!(await farmRepository.GetAllAsync()).Any())
                {
                    Console.WriteLine("Creando datos de ejemplo...");

                    var farm = new Farm { Id = Guid.NewGuid(), Name = "Granja Principal", Location = "Ubicación A" };
                    await farmRepository.AddAsync(farm);

                    var ponds = new Pond { Id = Guid.NewGuid(), Name = "Estanque 1", Capacity = 1000, FarmId = farm.Id };
                    await pondRepository.AddAsync(ponds);

                    var temperatureSensor = new Sensor
                    {
                        Id = Guid.NewGuid(),
                        VariableType = VariableType.Temperature,
                        PondId = ponds.Id
                    };
                    var levelSensor = new Sensor
                    {
                        Id = Guid.NewGuid(),
                        VariableType = VariableType.Level,
                        PondId = ponds.Id
                    };
                    await sensorRepository.AddAsync(temperatureSensor);
                    await sensorRepository.AddAsync(levelSensor);

                    var temperatureReading = new Reading
                    {
                        Id = Guid.NewGuid(),
                        Value = 28.5,
                        Timestamp = DateTime.UtcNow,
                        SensorId = temperatureSensor.Id
                    };
                    var levelReading = new Reading
                    {
                        Id = Guid.NewGuid(),
                        Value = 75.0,
                        Timestamp = DateTime.UtcNow,
                        SensorId = levelSensor.Id
                    };
                    await readingRepository.AddAsync(temperatureReading);
                    await readingRepository.AddAsync(levelReading);

                    Console.WriteLine("Datos de ejemplo creados correctamente.");
                }

                // Mostrar información por consola
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
                            Console.WriteLine($"    Sensor: {sensor.VariableType}");
                            var readings = await readingRepository.GetAllAsync();
                            foreach (var reading in readings.Where(r => r.SensorId == sensor.Id))
                            {
                                Console.WriteLine($"      Lectura: {reading.Value} ({reading.Timestamp})");
                            }
                        }
                    }
                }

                // Generar alertas
                Console.WriteLine("\nGenerando alertas...");
                var alerts = new List<Alert>();
                var allSensors = await sensorRepository.GetAllAsync();

                foreach (var sensor in allSensors)
                {
                    var readings = await readingRepository.GetAllAsync();
                    var latestReading = readings
                        .Where(r => r.SensorId == sensor.Id)
                        .OrderByDescending(r => r.Timestamp)
                        .FirstOrDefault();

                    if (latestReading != null)
                    {
                        if (sensor.VariableType == VariableType.Temperature && latestReading.Value > 30.0)
                        {
                            alerts.Add(new Alert("High temperature detected!", VariableType.Temperature, DateTime.UtcNow, sensor.Pond));
                        }
                        else if (sensor.VariableType == VariableType.Level && latestReading.Value < 50.0)
                        {
                            alerts.Add(new Alert("Low water level detected!", VariableType.Level, DateTime.UtcNow, sensor.Pond));
                        }
                    }
                }

                foreach (var alert in alerts)
                {
                    context.Alerts.Add(alert);
                }
                await context.SaveChangesAsync();
                Console.WriteLine($"{alerts.Count} alertas generadas y guardadas.");

                // Crear una alerta manualmente
                var pond = await context.Ponds.FirstOrDefaultAsync(); // Obtener un estanque existente
                if (pond != null)
                {
                    var manualAlert = new Alert
                    {
                        Id = Guid.NewGuid(),
                        Message = "Test alert: High temperature detected manually.",
                        TimeStamp = DateTime.UtcNow,
                        PondId = pond.Id,
                        Pond = pond,
                        VariableType = VariableType.Temperature
                    };

                    context.Alerts.Add(manualAlert);
                    await context.SaveChangesAsync();
                    Console.WriteLine("Alerta generada y guardada manualmente.");
                }
                else
                {
                    Console.WriteLine("No se encontró ningún estanque para asociar la alerta.");
                }
            }

            Console.WriteLine("\nPresiona cualquier tecla para salir...");
            Console.ReadKey();
        }
    }
}
