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
            // Verificar si hay una alerta activa para temperatura alta
            var alertaActiva = (await _alertRepository.GetActiveByTypeAsync(AlarmType.HighTemperature)).FirstOrDefault();

            // Validar la entrada
            if (float.IsNaN(temperatura) || float.IsInfinity(temperatura))
            {
                Console.WriteLine("Error: Temperatura inválida.");
                return;
            }

            if (temperatura > TemperaturaMuyAlta)
            {
                if (alertaActiva == null) // Solo crear una alerta si no existe una activa
                {
                    string mensaje = ConstruirMensaje(temperatura);
                    Console.WriteLine(mensaje);

                    var nuevaAlerta = CrearAlerta(mensaje, true);
                    await _alertRepository.AddAsync(nuevaAlerta);

                    // Enviar correo electrónico
                    _emailService.SendEmail(
                        "andyternblom@gmail.com",
                        "Alerta de Temperatura Alta",
                        mensaje
                    );
                }
                else
                {
                    Console.WriteLine("Ya existe una alerta activa de temperatura alta.");
                }
            }
            else
            {
                // Si la temperatura está dentro del rango normal, desactivar la alerta activa
                if (alertaActiva != null)
                {
                    alertaActiva.IsActive = false; // Desactivar la alerta
                    alertaActiva.TimeStamp = DateTime.UtcNow; // Actualizar el tiempo

                    await _alertRepository.UpdateAsync(alertaActiva);
                    Console.WriteLine("La alerta de temperatura alta ha sido desactivada.");
                }
            }
        }

        private string ConstruirMensaje(float temperatura)
        {
            return $"¡ALERTA! Temperatura muy alta: {temperatura:F2} °C";
        }

        private Alert CrearAlerta(string mensaje, bool isActive)
        {
            return new Alert
            {
                Id = Guid.NewGuid(),
                Message = mensaje,
                TimeStamp = DateTime.UtcNow,
                AlarmType = AlarmType.HighTemperature,
                IsActive = isActive,
            };
        }
    }
}
