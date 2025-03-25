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

            // Agregar servicios al contenedor
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Registrar el servicio de correo
            builder.Services.AddSingleton<EmailService>();

            builder.Services.AddDbContext<AquariusDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddScoped<IFarmRepository, FarmRepository>();
            builder.Services.AddScoped<IPondRepository, PondRepository>();
            builder.Services.AddScoped<ISensorRepository, SensorRepository>();
            builder.Services.AddScoped<IReadingRepository, ReadingRepository>();
            builder.Services.AddScoped<IAlertRepository, AlertRepository>();

            // Configuración de CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            // Middleware en el orden correcto
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting(); // UseRouting primero

            app.UseCors("AllowAngularApp"); // Aplicar CORS DESPUÉS de UseRouting

            app.UseAuthorization();

            app.MapControllers(); // UseEndpoints está implícito aquí con MapControllers()

            app.Run();

            // Crear el contexto de la base de datos y el repositorio
            var dbContext = new AquariusDbContext(); // Asegúrate de configurar tu DbContext correctamente
            var alertRepository = new AlertRepository(dbContext);

            // Crear instancias de las clases de alerta
            TemperaturaBaja alertaTemperaturaBaja = new TemperaturaBaja(alertRepository);
            TemperaturaAlta alertaTemperaturaAlta = new TemperaturaAlta(alertRepository);
            FaltaDeAgua alertaFaltaDeAgua = new FaltaDeAgua(alertRepository);

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

        static void ProcesarDatos(string data, TemperaturaBaja alertaTemperaturaBaja,TemperaturaAlta alertaTemperaturaAlta, FaltaDeAgua alertaFaltaDeAgua)
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
