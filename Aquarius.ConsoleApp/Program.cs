using Aquarius.Data;
using Aquarius.Data.Repositories;
using Aquarius.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Aquarius.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Configuración del servicio de inyección de dependencias
            var serviceProvider = new ServiceCollection()
                .AddDbContext<AquariusDbContext>(options =>
                    options.UseNpgsql("Host=localhost;Database=AquariusDB;Username=postgres;Password=1234"))
                .AddScoped<IFarmRepository, FarmRepository>()
                .AddScoped<IPondRepository, PondRepository>()
                .AddScoped<ITemperatureSensorRepository, TemperatureSensorRepository>()
                .AddScoped<ILevelSensorRepository, LevelSensorRepository>()
                .AddScoped<IReadingRepository, ReadingRepository>()
                .AddScoped<IAlertRepository, AlertRepository>()
                .BuildServiceProvider();

            // Crear datos iniciales y manejar la base de datos
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AquariusDbContext>();
                await context.Database.EnsureCreatedAsync(); // Crear la base de datos si no existe

                var farmRepository = scope.ServiceProvider.GetRequiredService<IFarmRepository>();
                var pondRepository = scope.ServiceProvider.GetRequiredService<IPondRepository>();
                var temperatureSensorRepository = scope.ServiceProvider.GetRequiredService<ITemperatureSensorRepository>();
                var levelSensorRepository = scope.ServiceProvider.GetRequiredService<ILevelSensorRepository>();
                var readingRepository = scope.ServiceProvider.GetRequiredService<IReadingRepository>();
                var alertRepository = scope.ServiceProvider.GetRequiredService<IAlertRepository>();

                // Crear y guardar datos si no existen
                if (!(await farmRepository.GetAllAsync()).Any())
                {
                    Console.WriteLine("Creando datos iniciales...");

                    // Crear una granja
                    var farm = new Farm
                    {
                        Id = Guid.NewGuid(),
                        Name = "Granja Principal",
                        Location = "Ubicación A"
                    };
                    await farmRepository.AddAsync(farm);

                    // Crear un estanque
                    var pond = new Pond
                    {
                        Id = Guid.NewGuid(),
                        Name = "Estanque Principal",
                        Capacity = 1000,
                        FarmId = farm.Id
                    };
                    await pondRepository.AddAsync(pond);

                    // Crear un sensor de temperatura
                    var temperatureSensor = new TemperatureSensor
                    {
                        Id = Guid.NewGuid(),
                        PondId = pond.Id
                    };
                    await temperatureSensorRepository.AddAsync(temperatureSensor);

                    // Crear un sensor de nivel con estado Full
                    var levelSensor = new LevelSensor
                    {
                        Id = Guid.NewGuid(),
                        FullPond = true,
                        PondId = pond.Id
                    };
                    await levelSensorRepository.AddAsync(levelSensor);


                    // Generar lecturas de temperatura aleatorias
                    var random = new Random();
                    for (int i = 0; i < 20; i++)
                    {
                        var temperatureValue = random.NextDouble() * (40 - 20) + 20;
                        var reading = new Reading
                        {
                            Id = Guid.NewGuid(),
                            Value = Math.Round(temperatureValue, 2),
                            Timestamp = DateTime.UtcNow.AddMinutes(-20 + i),
                            SensorId = temperatureSensor.Id
                        };
                        await readingRepository.AddAsync(reading);
                    }

                    // Crear las 3 alertas específicas
                    var alerts = new List<Alert>
                    {
                    new Alert("Temperatura alta", DateTime.UtcNow, pond, AlarmType.HighTemperature),
                    new Alert("Temperatura baja", DateTime.UtcNow, pond, AlarmType.LowTemperature),
                    new Alert("Desconexión de Arduino", DateTime.UtcNow, pond, AlarmType.Disconnection),
                    new Alert("Nivel inadecuado", DateTime.UtcNow, pond, AlarmType.LowLevel)
                    };


                    foreach (var alert in alerts)
                    {
                        await alertRepository.AddAsync(alert);
                    }

                    Console.WriteLine("Datos iniciales creados correctamente.");
                }

                // Mostrar datos existentes en la base de datos
                Console.WriteLine("\nInformación de la base de datos:");
                var farms = await farmRepository.GetAllAsync();
                foreach (var farm in farms)
                {
                    Console.WriteLine($"\nGranja: {farm.Name} ({farm.Location})");

                    var ponds = await pondRepository.GetAllAsync();
                    foreach (var pond in ponds.Where(p => p.FarmId == farm.Id))
                    {
                        Console.WriteLine($"  Estanque: {pond.Name} (Capacidad: {pond.Capacity})");

                        var levelSensors = await levelSensorRepository.GetAllAsync();
                        foreach (var sensor in levelSensors.Where(ls => ls.PondId == pond.Id))
                        {
                            Console.WriteLine($"    Sensor de Nivel: {(sensor.FullPond ? "Estanque lleno" : "Estanque vacío")}");
                        }

                        var temperatureSensors = await temperatureSensorRepository.GetAllAsync();
                        foreach (var sensor in temperatureSensors.Where(ts => ts.PondId == pond.Id))
                        {
                            Console.WriteLine($"    Sensor de Temperatura: {sensor.Id}");

                            var readings = await readingRepository.GetAllAsync();
                            foreach (var reading in readings.Where(r => r.SensorId == sensor.Id))
                            {
                                Console.WriteLine($"      Lectura: {reading.Value} °C (Registrada: {reading.Timestamp})");
                            }
                        }

                        var alerts = await alertRepository.GetAllAsync();
                        foreach (var alert in alerts.Where(a => a.PondId == pond.Id))
                        {
                            Console.WriteLine($"    Alerta: {alert.Message} ({alert.TimeStamp})");
                        }
                    }
                }
            }

            Console.WriteLine("\nPrograma finalizado.");
        }
    }
}
