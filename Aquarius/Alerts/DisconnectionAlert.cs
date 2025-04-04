using Aquarius.Data.Repositories;
using Aquarius.Domain;
using Aquarius.Services.Services;
using System.IO.Ports;

namespace Aquarius.Services.Alerts
{
    public class DisconnectionAlert
    {
        private readonly AlertRepository _alertRepository;
        private readonly EmailService _emailService;
        private readonly SerialPort _serialPort;

        public DisconnectionAlert(AlertRepository alertRepository, EmailService emailService)
        {
            _alertRepository = alertRepository ?? throw new ArgumentNullException(nameof(alertRepository));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));

            // Configuración del puerto serial
            _serialPort = new SerialPort("COM3", 9600);
            _serialPort.Open(); // Asegurarte de que el puerto esté abierto al iniciar
        }

        public async Task Verificar()
        {
            try
            {
                // Verificar si el puerto está abierto
                if (!_serialPort.IsOpen)
                {
                    var alertaActiva = (await _alertRepository.GetActiveByTypeAsync(AlarmType.Disconnection)).FirstOrDefault();

                    if (alertaActiva == null) // Solo crear una alerta si no existe una activa
                    {
                        string mensaje = "¡ALERTA! Arduino desconectado.";
                        Console.WriteLine(mensaje);

                        // Crear y guardar la alerta en la base de datos
                        var nuevaAlerta = CrearAlerta(mensaje, true);
                        await _alertRepository.AddAsync(nuevaAlerta);

                        // Enviar correo electrónico
                        _emailService.SendEmail(
                            "andyternblom@gmail.com",
                            "Alerta de desconexión del Arduino",
                            mensaje
                        );
                    }
                    else
                    {
                        Console.WriteLine("Ya existe una alerta activa de desconexión.");
                    }
                }
                else
                {
                    // Si el puerto está abierto, desactivar la alerta activa
                    var alertaActiva = (await _alertRepository.GetActiveByTypeAsync(AlarmType.Disconnection)).FirstOrDefault();
                    if (alertaActiva != null)
                    {
                        alertaActiva.IsActive = false; // Desactivar la alerta
                        alertaActiva.TimeStamp = DateTime.UtcNow; // Actualizar el tiempo

                        await _alertRepository.UpdateAsync(alertaActiva);
                        Console.WriteLine("La alerta de desconexión ha sido desactivada.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al verificar desconexión: {ex.Message}");
            }
        }

        private Alert CrearAlerta(string mensaje, bool isActive)
        {
            return new Alert
            {
                Id = Guid.NewGuid(),
                Message = mensaje,
                TimeStamp = DateTime.UtcNow,
                AlarmType = AlarmType.Disconnection,
                IsActive = isActive,
            };
        }
    }
}
