using Aquarius.Data;
using Aquarius.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.IO.Ports;
using Aquarius.Services.Alerts;
using Aquarius.Services.Services;


namespace Aquarius.Services
{
    public class Program
    {
        public static void Main(string[] args)
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

            // Crear el contexto de la base de datos y el repositorio
            var dbContext = new AquariusDbContext(); // Asegúrate de configurar tu DbContext correctamente
            var alertRepository = new AlertRepository(dbContext);
            var builders = WebApplication.CreateBuilder(args);
            IConfiguration configuration = builder.Configuration;

            var emailService = new EmailService(configuration);


            // Crear instancias de las clases de alerta
            LowTemperature alertaTemperaturaBaja = new LowTemperature(alertRepository, emailService);
            HighTemperature alertaTemperaturaAlta = new HighTemperature(alertRepository, emailService);
            WaterLevelAlert alertaFaltaDeAgua = new WaterLevelAlert(alertRepository, emailService);

            //Obtener los datos del Arduino

            // Configurar el puerto serial

            SerialPort serialPort = new SerialPort("COM3", 9600); // Cambia "COM3" por el puerto correcto

            serialPort.Open();

            while (true)
            {
                // Leer una línea de datos desde el puerto serial
                string data = serialPort.ReadLine();

                // Procesar los datos recibidos
                ProcesarDatos(data, alertaTemperaturaBaja, alertaTemperaturaAlta, alertaFaltaDeAgua);
            }
        }

        static void ProcesarDatos(string data, LowTemperature alertaTemperaturaBaja,HighTemperature alertaTemperaturaAlta, WaterLevelAlert alertaFaltaDeAgua)
        {
            // Dividir los datos en partes (temperatura y nivel)
            string[] partes = data.Split(',');

            if (partes.Length == 2)
            {
                try
                {
                    // Extraer el valor de temperatura (float)
                    string temperaturaStr = partes[0].Substring(2); // Eliminar "T:"
                    float temperatura = float.Parse(temperaturaStr);

                    // Extraer el valor de nivel (bool)
                    string nivelStr = partes[1].Substring(2); // Eliminar "L:"
                    bool nivel = nivelStr == "1"; // Convertir "1" a true y "0" a false
                                                  // Mostrar los valores en la consola
                    Console.WriteLine($"Temperatura: {temperatura:F2} °C, Nivel: {nivel}");

                    // Verificar alertas
                    alertaTemperaturaBaja.Verificar(temperatura);
                    alertaTemperaturaAlta.Verificar(temperatura);
                    alertaFaltaDeAgua.Verificar(nivel);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Error: Formato de datos incorrecto.");
                }
            }
            else
            {
                Console.WriteLine("Error: Datos incompletos.");
            }
        }
    }
}
