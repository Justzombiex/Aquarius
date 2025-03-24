using Aquarius.Data.Repositories;
using Aquarius.Domain;

namespace Aquarius.Services.Alerts
{
    public class TemperaturaBaja
    {
        private const float TemperaturaMuyBaja = 10.0f;
        private readonly AlertRepository _alertRepository;
        

        public TemperaturaBaja(AlertRepository alertRepository)
        {
            _alertRepository = alertRepository ?? throw new ArgumentNullException(nameof(alertRepository));
            
        }

        public async Task Verificar(float temperatura)
        {
            if (temperatura < TemperaturaMuyBaja)
            {
                string mensaje = $"¡ALERTA! Temperatura muy baja: {temperatura:F2} °C";
                Console.WriteLine(mensaje);

                // Crear y guardar la alerta en la base de datos
                var alerta = new Alert
                {
                    Id = Guid.NewGuid(),
                    Message = mensaje,
                    TimeStamp = DateTime.UtcNow,
                    VariableType = VariableType.Temperature // Tipo de variable
                };

                await _alertRepository.AddAsync(alerta);
            }
        }
    }
}
