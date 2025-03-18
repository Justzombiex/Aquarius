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
                        Name = "Estanque 1",
                        Capacity = 1000,
                        FarmId = farm.Id
                    };
                    await pondRepository.AddAsync(pond);

                    // Crear sensores
                    var temperatureSensor = new Sensor
                    {
                        Id = Guid.NewGuid(),
                        VariableType = VariableType.Temperature,
                        PondId = pond.Id
                    };
                    var levelSensor = new Sensor
                    {
                        Id = Guid.NewGuid(),
                        VariableType = VariableType.Level,
                        PondId = pond.Id
                    };
                    await sensorRepository.AddAsync(temperatureSensor);
                    await sensorRepository.AddAsync(levelSensor);

                    // Crear lecturas
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
                    foreach (var pond in ponds.Where(p => p.FarmId == farm.Id))
                    {
                        Console.WriteLine($"  Estanque: {pond.Name} (Capacidad: {pond.Capacity})");

                        var sensors = await sensorRepository.GetAllAsync();
                        foreach (var sensor in sensors.Where(s => s.PondId == pond.Id))
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
            }

            Console.WriteLine("\nPresiona cualquier tecla para salir...");
            Console.ReadKey();
        }
    }
}