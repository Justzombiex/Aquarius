using Aquarius.Data;
using Aquarius.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.IO.Ports;
using Aquarius.Services.Alerts;
using Aquarius.Services.Services;
using Aquarius.Domain;


namespace Aquarius.Services
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Servicios del Contenedor
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            // Registrar el servicio de correo
            builder.Services.AddSingleton<EmailService>();

            builder.Services.AddDbContext<AquariusDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            // Inyección de Dependencias (Repositorios)
            builder.Services.AddScoped<IFarmRepository, FarmRepository>();
            builder.Services.AddScoped<IPondRepository, PondRepository>();
            builder.Services.AddScoped<ITemperatureSensorRepository, TemperatureSensorRepository>();
            builder.Services.AddScoped<IReadingRepository, ReadingRepository>();
            builder.Services.AddScoped<IAlertRepository, AlertRepository>();
            builder.Services.AddScoped<ILevelSensorRepository, LevelSensorRepository>();

            // Configuración de CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp", policy =>
                {
                    policy.WithOrigins("http://localhost:4200") // URL de la app Angular
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials(); // Permite cookies o autenticación si es necesario
                });
            });

            // Construcción del Aplicativo
            var app = builder.Build();

            // Middleware
            if (app.Environment.IsDevelopment())
            {
                // Documentación Swagger habilitada en entorno de desarrollo
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Redirección HTTPS (puedes comentar esto para desarrollo con HTTP)
            app.UseHttpsRedirection();

            // Configuración de Routing y CORS
            app.UseRouting(); // Routing
            app.UseCors("AllowAngularApp"); // Aplicar política de CORS después de Routing

            // Autorización
            app.UseAuthorization();

            // Mapear Controladores
            app.MapControllers();

            // Ejecución de la Aplicación
            app.Run();

            using var scope = app.Services.CreateScope();
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
            var levelSensorRepository = services.GetRequiredService<ILevelSensorRepository>();
            var levelSensor = (await levelSensorRepository.GetAllAsync()).FirstOrDefault();
            if (levelSensor == null)
            {
                levelSensor = new LevelSensor(true, pond); // Pond referenciado
                await levelSensorRepository.AddAsync(levelSensor);
                Console.WriteLine("Sensor de Nivel creado: FullPond = true");
            }

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
            SerialPort serialPort = new SerialPort("COM3", 9600);
            serialPort.Open();

            var dataProcessorService = services.GetRequiredService<DataProcessorService>();

            // Leer y procesar datos del puerto serial
            while (true)
            {
                string data = serialPort.ReadLine();
                await dataProcessorService.ProcessDataAsync(data);
            }
        }
    }
}
