using Aquarius.Data.Repositories;
using Aquarius.Domain;
using Aquarius.Services.Services;

namespace Aquarius.Services.Alerts
{
    public class HighTemperature
    {
        private const float TemperaturaMuyAlta = 40.0f;
        private readonly AlertRepository _alertRepository;
        private readonly EmailService _emailService;


        public HighTemperature(AlertRepository alertRepository, EmailService emailService)
        {
            _alertRepository = alertRepository ?? throw new ArgumentNullException(nameof(alertRepository));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public async Task Verificar(float temperatura)
        {
            if (temperatura > TemperaturaMuyAlta)
            {
                string mensaje = $"¡ALERTA! Temperatura muy alta: {temperatura:F2} °C";
                Console.WriteLine(mensaje);

                // Crear y guardar la alerta en la base de datos
                var alerta = new Alert
                {
                    Id = Guid.NewGuid(),
                    Message = mensaje,
                    TimeStamp = DateTime.UtcNow,
                    IsActive = true,
                };

                await _alertRepository.AddAsync(alerta);
                // Enviar correo electrónico
                _emailService.SendEmail("andyternblom@gmail.com", "Alerta de Temperatura Alta", mensaje);
            }
        }
    }
}
