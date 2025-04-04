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
            // Verificar si hay una alerta activa para nivel bajo de agua
            var alertaActiva = (await _alertRepository.GetActiveByTypeAsync(AlarmType.LowLevel)).FirstOrDefault();

            if (!nivel) // Si el nivel es bajo (false)
            {
                if (alertaActiva == null) // Solo crear una alerta si no existe una activa
                {
                    string mensaje = "¡ALERTA! Falta agua.";
                    Console.WriteLine(mensaje);

                    var nuevaAlerta = CrearAlerta(mensaje, true);
                    await _alertRepository.AddAsync(nuevaAlerta);

                    // Enviar correo electrónico
                    _emailService.SendEmail(
                        "andyternblom@gmail.com",
                        "Alerta de Falta de Agua",
                        mensaje
                    );
                }
                else
                {
                    Console.WriteLine("Ya existe una alerta activa de falta de agua.");
                }
            }
            else
            {
                // Si el nivel de agua es adecuado, desactivar la alerta activa
                if (alertaActiva != null)
                {
                    alertaActiva.IsActive = false; // Desactivar la alerta
                    alertaActiva.TimeStamp = DateTime.UtcNow; // Actualizar el tiempo

                    await _alertRepository.UpdateAsync(alertaActiva);
                    Console.WriteLine("La alerta de falta de agua ha sido desactivada.");
                }
            }
        }

        private Alert CrearAlerta(string mensaje, bool isActive)
        {
            return new Alert
            {
                Id = Guid.NewGuid(),
                Message = mensaje,
                TimeStamp = DateTime.UtcNow,
                AlarmType = AlarmType.LowLevel,
                IsActive = isActive,
            };
        }
    }
}
