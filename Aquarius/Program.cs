using Aquarius.Data;
using Aquarius.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Aquarius.Services
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<AquariusDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddScoped<IFarmRepository, FarmRepository>();
            builder.Services.AddScoped<IPondRepository, PondRepository>();
            builder.Services.AddScoped<ISensorRepository, SensorRepository>();
            builder.Services.AddScoped<IReadingRepository, ReadingRepository>();
            builder.Services.AddScoped<IAlertRepository, AlertRepository>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
