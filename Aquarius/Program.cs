using Aquarius.Data;
using Aquarius.Data.Repositories;
using Aquarius.Domain;
using Aquarius.Services.Services;
using Microsoft.EntityFrameworkCore;
using System.IO.Ports;


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

            app.Run();

        }
    }
}
