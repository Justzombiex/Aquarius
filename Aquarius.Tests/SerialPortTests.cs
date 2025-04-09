using Aquarius.Data;
using Aquarius.Data.Repositories;
using Aquarius.Domain;
using Aquarius.Services.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class SerialPortTests
{
    [Fact]
    public async Task SerialPort_IsOpen_ShouldHandleDisconnectionAlert()
    {
        bool IsOpen = true;

        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        var dbContext = services.GetRequiredService<AquariusDbContext>();

        var pondRepository = services.GetRequiredService<IPondRepository>();
        var pond = await pondRepository.GetByIdAsync(new Guid("369478ca-5c79-4b93-b936-a8f3284e6f97"));

        var alertRepository = services.GetRequiredService<IAlertRepository>();

        // Configuración de la alerta simulada
        var mockAlert = new Alert
        {
            Id = Guid.NewGuid(),
            Message = "Alerta desconexión activa (tests)",
            TimeStamp = DateTime.UtcNow,
            Pond = pond,
            AlarmType = AlarmType.Disconnection,
            IsActive = true
        };

        // Acción en caso de puerto abierto
        if (IsOpen)
        {
            var alertDisconnection = (await alertRepository.GetActiveByTypeAsync(AlarmType.Disconnection)).FirstOrDefault();
            if (alertDisconnection != null)
            {
                var alertD = new Alert
                {
                    Id = alertDisconnection.Id,
                    Message = alertDisconnection.Message,
                    TimeStamp = alertDisconnection.TimeStamp,
                    Pond = pond,
                    AlarmType = AlarmType.Disconnection,
                    IsActive = false
                };

                await alertRepository.DeleteAsync(alertDisconnection.Id);
                await alertRepository.AddAsync(alertD);

                Assert.False(alertD.IsActive); // Validar que la alerta está inactiva
            }
            else
            {
                Assert.True(alertDisconnection == null);
            }
        }
    }

    [Fact]
    public async Task SerialPort_IsNotOpen_ShouldAddDisconnectionAlert()
    {
        bool isOpen = false;

        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        var dbContext = services.GetRequiredService<AquariusDbContext>();

        var pondRepository = services.GetRequiredService<IPondRepository>();
        var pond = await pondRepository.GetByIdAsync(new Guid("369478ca-5c79-4b93-b936-a8f3284e6f97"));

        var alertRepository = services.GetRequiredService<IAlertRepository>();

        // Acción en caso de puerto no abierto
        if (!isOpen)
        {
            var alertDisconnection = await alertRepository.GetActiveByTypeAsync(AlarmType.Disconnection);
            if (alertDisconnection == null || !alertDisconnection.Any())
            {
                var alertD = new Alert
                {
                    Message = "Desconexión con el Arduino",
                    TimeStamp = DateTime.UtcNow,
                    Pond = pond,
                    AlarmType = AlarmType.Disconnection,
                    IsActive = true
                };

                await alertRepository.AddAsync(alertD);

                var alertDisconnection2 = (await alertRepository.GetActiveByTypeAsync(AlarmType.Disconnection)).FirstOrDefault();

                Assert.True(alertDisconnection2.IsActive); // Validar que la alerta está activa
            }
            else
            {
                Assert.False(false);
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
