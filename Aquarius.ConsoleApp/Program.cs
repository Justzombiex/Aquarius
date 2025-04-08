using Aquarius.Data;
using Aquarius.Data.Repositories;
using Aquarius.Domain;
using Aquarius.Services.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.IO.Ports;

namespace Aquarius.ConsoleApp
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            // Configurar servicios y dependencias
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            // Construcción del ServiceProvider
            var serviceProvider = serviceCollection.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            var dbContext = services.GetRequiredService<AquariusDbContext>();

            // Verificar y crear Farm
            var farmRepository = services.GetRequiredService<IFarmRepository>();
            var farm = (await farmRepository.GetAllAsync()).FirstOrDefault();
            if (farm == null)
            {
                farm = new Farm("Granja Principal", "Ubicación Principal");
                await farmRepository.AddAsync(farm);
                Console.WriteLine("Granja creada: Granja Principal");
            }

            // Verificar y crear Pond
            var pondRepository = services.GetRequiredService<IPondRepository>();
            var pond = (await pondRepository.GetAllAsync()).FirstOrDefault();
            if (pond == null)
            {
                pond = new Pond("Estanque Principal", 5000, farm); // Capacidad arbitraria de 5000
                await pondRepository.AddAsync(pond);
                Console.WriteLine("Estanque creado: Estanque Principal");
            }

            // Verificar y crear LevelSensor


            // Verificar y crear TemperatureSensor
            var temperatureSensorRepository = services.GetRequiredService<ITemperatureSensorRepository>();
            var temperatureSensor = (await temperatureSensorRepository.GetAllAsync()).FirstOrDefault();
            if (temperatureSensor == null)
            {
                temperatureSensor = new TemperatureSensor(pond); // Pond referenciado
                await temperatureSensorRepository.AddAsync(temperatureSensor);
                Console.WriteLine("Sensor de Temperatura creado");
            }

            // Configurar el puerto serial
            try
            {
                SerialPort serialPort = new SerialPort("COM3", 9600);
                serialPort.Open();

                if (serialPort.IsOpen)
                {
                    Console.WriteLine("Puerto abierto");
                    var alertRepository = services.GetRequiredService<IAlertRepository>();
                    var alertDisconnection = (await alertRepository.GetActiveByTypeAsync(AlarmType.Disconnection)).FirstOrDefault();
                    if (alertDisconnection != null)
                    {
                        var alertD = new Alert
                        {
                            Id = alertDisconnection.Id,
                            Message = alertDisconnection.Message,          // Mensaje de la alerta de desconexión
                            TimeStamp = alertDisconnection.TimeStamp,      // Marca de tiempo de la alerta
                            Pond = pond,                                   // Referencia al estanque
                            AlarmType = AlarmType.Disconnection,           // Tipo de alarma: Desconexión
                            IsActive = false                               // Configuración para isActive
                        };
                        await alertRepository.DeleteAsync(alertDisconnection.Id);
                        await alertRepository.AddAsync(alertD);
                    }
                    else
                    {
                        Console.WriteLine("No hay alerta de desconcexión activa");
                    }
                }
                else
                {
                    Console.WriteLine("Puerto no abierto");
                    var alertRepository = services.GetRequiredService<IAlertRepository>();
                    var alertDisconnection = await alertRepository.GetActiveByTypeAsync(AlarmType.Disconnection);
                    if (alertDisconnection == null || !alertDisconnection.Any())
                    {
                        var alertD = new Alert
                        {
                            Message = "Desconexión con el Arduino",          // Mensaje de la alerta de desconexión
                            TimeStamp = DateTime.UtcNow,      // Marca de tiempo de la alerta
                            Pond = pond,                                   // Referencia al estanque
                            AlarmType = AlarmType.Disconnection,           // Tipo de alarma: Desconexión
                            IsActive = true                               // Configuración para isActive
                        };
                        await alertRepository.AddAsync(alertD);
                    }
                    else
                    {
                        Console.WriteLine("Ya hay una alerta de desconexión activa");
                    }
                }


                while (true)
                {
                    serialPort.ReadTimeout = 3000;

                    try
                    {
                        // Leer el nivel (primera línea)
                        string nivelData = serialPort.ReadLine().Trim(); // Leer y limpiar la línea
                        bool nivel = nivelData == "1"; // Convertir "1" a true y "0" a false
                        Console.WriteLine($"Nivel: {nivel}");

                        if (nivel == true)
                        {
                            var alertRepository = services.GetRequiredService<IAlertRepository>();
                            var alertLevel = (await alertRepository.GetActiveByTypeAsync(AlarmType.LowLevel)).FirstOrDefault();
                            if (alertLevel != null)
                            {
                                var alertL = new Alert
                                {
                                    Id = alertLevel.Id,
                                    Message = alertLevel.Message,          // Mensaje de la alerta de desconexión
                                    TimeStamp = alertLevel.TimeStamp,      // Marca de tiempo de la alerta
                                    Pond = pond,                                   // Referencia al estanque
                                    AlarmType = AlarmType.Disconnection,           // Tipo de alarma: Desconexión
                                    IsActive = false                               // Configuración para isActive
                                };
                                await alertRepository.DeleteAsync(alertLevel.Id);
                                await alertRepository.AddAsync(alertL);
                            }
                            else
                            {
                                Console.WriteLine("Todo bien con la alerta de nivel");
                            }
                        }
                        else
                        {
                            var alertRepository = services.GetRequiredService<IAlertRepository>();
                            var alertLevel = (await alertRepository.GetActiveByTypeAsync(AlarmType.LowLevel)).FirstOrDefault();
                            if (alertLevel != null)
                            {
                                Console.WriteLine("Hay alerta de nivel activa");
                            }
                            else
                            {
                                var alertL = new Alert
                                {
                                    Message = "Estanque seco",         // Mensaje de la alerta de desconexión
                                    TimeStamp = DateTime.UtcNow,      // Marca de tiempo de la alerta
                                    Pond = pond,                                   // Referencia al estanque
                                    AlarmType = AlarmType.LowLevel,           // Tipo de alarma: Desconexión
                                    IsActive = true                              // Configuración para isActive
                                };
                                await alertRepository.AddAsync(alertL);
                            }
                        }

                        var levelSensorRepository = services.GetRequiredService<ILevelSensorRepository>();
                        var levelSensor = (await levelSensorRepository.GetAllAsync()).FirstOrDefault();
                        await levelSensorRepository.DeleteAsync(levelSensor.Id);
                        await levelSensorRepository.AddAsync(levelSensor);

                        // Leer la temperatura (segunda línea)
                        string tempData = serialPort.ReadLine().Trim(); // Leer y limpiar la línea

                        // Utilizar CultureInfo para asegurar el formato correcto
                        if (float.TryParse(tempData, NumberStyles.Float, CultureInfo.InvariantCulture, out float temperatura))
                        {
                            Console.WriteLine($"Temperatura: {temperatura}");

                            var reading = new Reading(temperatura, DateTime.UtcNow, temperatureSensor);
                            var readingRepository = services.GetRequiredService<IReadingRepository>();
                            await readingRepository.AddAsync(reading);
                        }
                        else
                        {
                            Console.WriteLine("Error al convertir la temperatura.");
                        }

                        if (temperatura <= 33)
                        {
                            var alertRepository = services.GetRequiredService<IAlertRepository>();
                            var alertHi = (await alertRepository.GetActiveByTypeAsync(AlarmType.HighTemperature)).FirstOrDefault();
                            if (alertHi != null)
                            {
                                var alertH = new Alert
                                {
                                    Id = alertHi.Id,
                                    Message = alertHi.Message,          // Mensaje de la alerta de desconexión
                                    TimeStamp = alertHi.TimeStamp,      // Marca de tiempo de la alerta
                                    Pond = pond,                                   // Referencia al estanque
                                    AlarmType = AlarmType.HighTemperature,           // Tipo de alarma: Desconexión
                                    IsActive = false                               // Configuración para isActive
                                };
                                await alertRepository.DeleteAsync(alertHi.Id);
                                await alertRepository.AddAsync(alertH);
                            }
                            else
                            {
                                Console.WriteLine("Todo bien con la alerta de temperatura alta");
                            }
                        }
                        else
                        {
                            var alertRepository = services.GetRequiredService<IAlertRepository>();
                            var alertHi = (await alertRepository.GetActiveByTypeAsync(AlarmType.HighTemperature)).FirstOrDefault();
                            if (alertHi != null)
                            {
                                Console.WriteLine("Hay alerta de temperatura alta activa");
                            }
                            else
                            {
                                var alertH = new Alert
                                {
                                    Message = "Temperatura alta",         // Mensaje de la alerta de desconexión
                                    TimeStamp = DateTime.UtcNow,      // Marca de tiempo de la alerta
                                    Pond = pond,                                   // Referencia al estanque
                                    AlarmType = AlarmType.HighTemperature,           // Tipo de alarma: Desconexión
                                    IsActive = true                              // Configuración para isActive
                                };
                                await alertRepository.AddAsync(alertH);
                            }
                        }

                        if (temperatura >= 24)
                        {
                            var alertRepository = services.GetRequiredService<IAlertRepository>();
                            var alertLo = (await alertRepository.GetActiveByTypeAsync(AlarmType.LowTemperature)).FirstOrDefault();
                            if (alertLo != null)
                            {
                                var alertLow = new Alert
                                {
                                    Id = alertLo.Id,
                                    Message = alertLo.Message,          // Mensaje de la alerta de desconexión
                                    TimeStamp = alertLo.TimeStamp,      // Marca de tiempo de la alerta
                                    Pond = pond,                                   // Referencia al estanque
                                    AlarmType = AlarmType.LowTemperature,           // Tipo de alarma: Desconexión
                                    IsActive = false                               // Configuración para isActive
                                };
                                await alertRepository.DeleteAsync(alertLo.Id);
                                await alertRepository.AddAsync(alertLow);
                            }
                            else
                            {
                                Console.WriteLine("Todo bien con la alerta de baja");
                            }
                        }
                        else
                        {
                            var alertRepository = services.GetRequiredService<IAlertRepository>();
                            var alertLo = (await alertRepository.GetActiveByTypeAsync(AlarmType.LowTemperature)).FirstOrDefault();
                            if (alertLo != null)
                            {
                                Console.WriteLine("Hay alerta de temperatura baja activa");
                            }
                            else
                            {
                                var alertL = new Alert
                                {
                                    Message = "Temperatura baja",         // Mensaje de la alerta de desconexión
                                    TimeStamp = DateTime.UtcNow,      // Marca de tiempo de la alerta
                                    Pond = pond,                                   // Referencia al estanque
                                    AlarmType = AlarmType.LowTemperature,           // Tipo de alarma: Desconexión
                                    IsActive = true                              // Configuración para isActive
                                };
                                await alertRepository.AddAsync(alertL);
                            }
                        }

                    }
                    catch (TimeoutException)
                    {
                        Console.WriteLine("No se recibieron datos en el tiempo esperado.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al procesar los datos: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al abrir el puerto serial: {ex.Message}");
                // El programa continúa incluso si el puerto no se abre correctamente
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Registrar servicios y repositorios
            services.AddDbContext<AquariusDbContext>(options =>
                options.UseNpgsql("Host=localhost;Database=AquariusDB;Username=postgres;Password=1234"));

            services.AddScoped<IFarmRepository, FarmRepository>();
            services.AddScoped<IPondRepository, PondRepository>();
            services.AddScoped<ILevelSensorRepository, LevelSensorRepository>();
            services.AddScoped<ITemperatureSensorRepository, TemperatureSensorRepository>();
            services.AddScoped<IReadingRepository, ReadingRepository>();
            services.AddScoped<IAlertRepository, AlertRepository>();

            services.AddTransient<DataProcessorService>();
        }

    }
}
