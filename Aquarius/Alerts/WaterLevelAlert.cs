using Aquarius.Data.Repositories;
using Aquarius.Domain;
using Aquarius.Services.Services;

namespace Aquarius.Services.Alerts
{
    public class WaterLevelAlert
    {
        private readonly AlertRepository _alertRepository;
        private readonly EmailService _emailService;

        public WaterLevelAlert(AlertRepository alertRepository, EmailService emailService)
        {
            _alertRepository = alertRepository ?? throw new ArgumentNullException(nameof(alertRepository));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public async Task Verificar(bool nivel)
        {
            if (!nivel) // Si el nivel es false (0), falta agua
            {
                string mensaje = "¡ALERTA! Falta agua.";
                Console.WriteLine(mensaje);

                // Crear y guardar la alerta en la base de datos
                var alerta = new Alert
                {
                    Id = Guid.NewGuid(),
                    Message = mensaje,
                    TimeStamp = DateTime.UtcNow,
                };

                await _alertRepository.AddAsync(alerta);
                // Enviar correo electrónico
                _emailService.SendEmail("andyternblom@gmail.com", "Alerta de Falta de Agua", mensaje);
            }
        }
    }
}
