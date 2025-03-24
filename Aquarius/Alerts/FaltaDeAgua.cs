using Aquarius.Data.Repositories;
using Aquarius.Domain;

namespace Aquarius.Services.Alerts
{
    public class FaltaDeAgua
    {
        private readonly AlertRepository _alertRepository;
        

        public FaltaDeAgua(AlertRepository alertRepository)
        {
            _alertRepository = alertRepository ?? throw new ArgumentNullException(nameof(alertRepository));
            
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
                    VariableType = VariableType.Level // Tipo de variable
                };

                await _alertRepository.AddAsync(alerta);
            }
        }
    }
}
