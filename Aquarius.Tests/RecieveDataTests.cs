using Aquarius.Data;
using Aquarius.Data.Repositories;
using Aquarius.Domain;
using Aquarius.Services.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

public class RecieveDataTests
{
    [Theory]
    [InlineData("123.42")] // Formato: primer dígito es el nivel, el resto es la temperatura con decimales
    public async Task Data_ShouldSaveCorrectly1(string data)
    {
        // Procesar el nivel (primer dígito)
        string nivelData = data.Substring(0, 1); // Extraer el primer carácter como nivel
        bool nivel = nivelData == "1";
        Assert.True(nivel);

        // Procesar la temperatura (resto de los dígitos)
        string tempData = data.Substring(1).Trim(); // Extraer la temperatura desde el segundo carácter
        float temp;
        bool tempC = float.TryParse(tempData, NumberStyles.Float, CultureInfo.InvariantCulture, out temp);
        Assert.True(tempC);

        // Comparar directamente con 23.42
        Assert.True(Math.Round(temp, 2) == 23.42);
    }

    [Theory]
    [InlineData("023.43")] // Formato: primer dígito es el nivel, el resto es la temperatura con decimales
    public async Task Data_ShouldSaveCorrectly0(string data)
    {
        // Procesar el nivel (primer dígito)
        string nivelData = data.Substring(0, 1); // Extraer el primer carácter como nivel
        bool nivel = nivelData == "1";
        Assert.False(nivel);

        // Procesar la temperatura (resto de los dígitos)
        string tempData = data.Substring(1).Trim(); // Extraer la temperatura desde el segundo carácter
        float temp;
        bool tempC = float.TryParse(tempData, NumberStyles.Float, CultureInfo.InvariantCulture, out temp);
        Assert.True(tempC);

        // Comparar directamente con 23.42
        Assert.True(Math.Round(temp, 2) == 23.43);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task LevelAlert_ShouldBeActivated(bool level)
    {

        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        var dbContext = services.GetRequiredService<AquariusDbContext>();

        var pondRepository = services.GetRequiredService<IPondRepository>();
        var pond = await pondRepository.GetByIdAsync(new Guid("369478ca-5c79-4b93-b936-a8f3284e6f97"));

        var alertRepository = services.GetRequiredService<IAlertRepository>();
        var alertLevel = (await alertRepository.GetActiveByTypeAsync(AlarmType.LowLevel)).FirstOrDefault();

        if (level)
        {
            if (alertLevel != null)
            {
                var alertL = new Alert
                {
                    Id = alertLevel.Id,
                    Message = alertLevel.Message,          // Mensaje de la alerta de desconexión
                    TimeStamp = alertLevel.TimeStamp,      // Marca de tiempo de la alerta
                    Pond = pond,                                   // Referencia al estanque
                    AlarmType = AlarmType.LowLevel,           // Tipo de alarma: Desconexión
                    IsActive = false                               // Configuración para isActive
                };
                await alertRepository.DeleteAsync(alertLevel.Id);
                await alertRepository.AddAsync(alertL);
                var alertA = (await alertRepository.GetActiveByTypeAsync(AlarmType.LowLevel)).FirstOrDefault();
                Assert.True(alertA == null);
            }
            else
            {
                Console.WriteLine("Todo bien con la alerta de nivel");
                Assert.True(alertLevel == null);
            }
        }
        else
        {
            if (alertLevel != null)
            {
                Console.WriteLine("Hay alerta de nivel activa");
                Assert.True(!alertLevel.IsActive);
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
                var alertA = (await alertRepository.GetActiveByTypeAsync(AlarmType.LowLevel)).FirstOrDefault();
                Assert.True(alertA.IsActive);
            }
        }

    }

    [Theory]
    [InlineData("25.45")]
    [InlineData("26.0")]
    [InlineData("27")]
    public async Task Temperature_ShouldBeSaved(string temperature)
    {

        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        var dbContext = services.GetRequiredService<AquariusDbContext>();


        var pondRepository = services.GetRequiredService<IPondRepository>();
        var pond = await pondRepository.GetByIdAsync(new Guid("369478ca-5c79-4b93-b936-a8f3284e6f97"));

        var temperatureSensorRepository = services.GetRequiredService<ITemperatureSensorRepository>();
        var temperatureSensor = (await temperatureSensorRepository.GetAllAsync()).FirstOrDefault();
        if (temperatureSensor == null)
        {
            temperatureSensor = new TemperatureSensor(pond); // Pond referenciado
            await temperatureSensorRepository.AddAsync(temperatureSensor);
            Console.WriteLine("Sensor de Temperatura creado");
        }


        if (float.TryParse(temperature, NumberStyles.Float, CultureInfo.InvariantCulture, out float temperatura))
        {
            Console.WriteLine($"Temperatura: {temperatura}");

            var reading = new Reading(temperatura, DateTime.UtcNow, temperatureSensor);
            var readingRepository = services.GetRequiredService<IReadingRepository>();

            await readingRepository.AddAsync(reading);

            Assert.True(true);
        }
        else
        {
            Console.WriteLine("Error al convertir la temperatura.");
        }
    }

    [Theory]
    [InlineData("39")]
    [InlineData("1")]
    [InlineData("27")]
    public async Task AlertTemperature_ShouldActivate(string temperature)
    {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        var dbContext = services.GetRequiredService<AquariusDbContext>();


        var pondRepository = services.GetRequiredService<IPondRepository>();
        var pond = await pondRepository.GetByIdAsync(new Guid("369478ca-5c79-4b93-b936-a8f3284e6f97"));

        var temperatureSensorRepository = services.GetRequiredService<ITemperatureSensorRepository>();
        var temperatureSensor = (await temperatureSensorRepository.GetAllAsync()).FirstOrDefault();
        if (temperatureSensor == null)
        {
            temperatureSensor = new TemperatureSensor(pond); // Pond referenciado
            await temperatureSensorRepository.AddAsync(temperatureSensor);
            Console.WriteLine("Sensor de Temperatura creado");
        }


        if (float.TryParse(temperature, NumberStyles.Float, CultureInfo.InvariantCulture, out float temperatura))
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
                var alertHis = (await alertRepository.GetActiveByTypeAsync(AlarmType.HighTemperature)).FirstOrDefault();
                Assert.True(alertHis == null);
            }
            else
            {
                Console.WriteLine("Todo bien con la alerta de temperatura alta");
                Assert.True(alertHi == null);
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
                var alertHis = (await alertRepository.GetActiveByTypeAsync(AlarmType.HighTemperature)).FirstOrDefault();
                Assert.True(alertHis.IsActive);
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
                var alertLos = (await alertRepository.GetActiveByTypeAsync(AlarmType.LowTemperature)).FirstOrDefault();
                Assert.True(alertLos == null);
            }
            else
            {
                Console.WriteLine("Todo bien con la alerta de baja");
                Assert.True(temperatura >= 24);
            }
        }
        else
        {
            var alertRepository = services.GetRequiredService<IAlertRepository>();
            var alertLo = (await alertRepository.GetActiveByTypeAsync(AlarmType.LowTemperature)).FirstOrDefault();
            if (alertLo != null)
            {
                Console.WriteLine("Hay alerta de temperatura baja activa");
                Assert.True(temperatura >= 24);
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
                var alertLos = (await alertRepository.GetActiveByTypeAsync(AlarmType.LowTemperature)).FirstOrDefault();
                Assert.True(alertLos.IsActive);
            }
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
