using Aquarius.Data.Repositories;
using Aquarius.Domain;

namespace Aquarius.Services.Alerts
{
    public class TemperaturaAlta
    {
        private const float TemperaturaMuyAlta = 40.0f;
        private readonly AlertRepository _alertRepository;
        

        public TemperaturaAlta(AlertRepository alertRepository)
        {
            _alertRepository = alertRepository ?? throw new ArgumentNullException(nameof(alertRepository));
           
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
                    VariableType = VariableType.Temperature // Tipo de variable
                };

                await _alertRepository.AddAsync(alerta);
            }
        }
    }
}
