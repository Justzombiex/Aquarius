using Aquarius.Services.Alerts;
using Aquarius.Domain;
using Aquarius.Data.Repositories;

namespace Aquarius.Services.Services
{
    public class DataProcessorService
    {
        private readonly LowTemperature _lowTemperatureAlert;
        private readonly HighTemperature _highTemperatureAlert;
        private readonly WaterLevelAlert _waterLevelAlert;
        private readonly DisconnectionAlert _disconnectionAlert;
        private readonly ReadingRepository _readingRepository;
        private readonly LevelSensorRepository _levelSensorRepository;
        private readonly TemperatureSensor _temperatureSensor;

        public DataProcessorService(
            LowTemperature lowTemperatureAlert,
            HighTemperature highTemperatureAlert,
            WaterLevelAlert waterLevelAlert,
            DisconnectionAlert disconnectionAlert,
            ReadingRepository readingRepository,
            LevelSensorRepository levelSensorRepository,
            TemperatureSensor temperatureSensor)
        {
            _lowTemperatureAlert = lowTemperatureAlert;
            _highTemperatureAlert = highTemperatureAlert;
            _waterLevelAlert = waterLevelAlert;
            _disconnectionAlert = disconnectionAlert;
            _readingRepository = readingRepository ?? throw new ArgumentNullException(nameof(readingRepository));
            _levelSensorRepository = levelSensorRepository ?? throw new ArgumentNullException(nameof(levelSensorRepository));
            _temperatureSensor = temperatureSensor ?? throw new ArgumentNullException(nameof(temperatureSensor));
        }

        public async Task ProcessDataAsync(string data)
        {
            try
            {
                // Dividir los datos en partes con delimitadores de ":" y ","
                string[] partes = data.Split(new[] { ',', ':' }, StringSplitOptions.RemoveEmptyEntries);

                if (partes.Length == 4 && partes[0] == "Nivel" && partes[2] == "Temperatura")
                {
                    // Extraer el valor de nivel (bool)
                    string nivelStr = partes[1].Trim();
                    bool nivel = nivelStr == "1"; // Convertir "1" a true y "0" a false

                    // Extraer el valor de temperatura (float)
                    string temperaturaStr = partes[3].Trim();
                    float temperatura = float.Parse(temperaturaStr);

                    // Mostrar los valores en la consola
                    Console.WriteLine($"Temperatura: {temperatura:F2} °C, Nivel: {nivel}");

                    // Guardar la temperatura en la base de datos
                    var nuevaLectura = new Reading
                    {
                        Id = Guid.NewGuid(),
                        Value = temperatura,
                        Timestamp = DateTime.UtcNow,
                        SensorId = _temperatureSensor.Id,
                        Sensor = _temperatureSensor
                    };

                    await _readingRepository.AddAsync(nuevaLectura);
                    Console.WriteLine($"Temperatura guardada: {temperatura:F2} °C en la base de datos.");

                    // Actualizar el estado del sensor de nivel en la base de datos
                    var levelSensors = await _levelSensorRepository.GetAllAsync(); // Obtener todos los sensores
                    var levelSensor = levelSensors.FirstOrDefault(); // Asumir que hay solo uno
                    if (levelSensor != null)
                    {
                        levelSensor.FullPond = nivel; // Actualizar el valor de nivel
                        await _levelSensorRepository.UpdateAsync(levelSensor);
                        Console.WriteLine($"Estado del sensor de nivel actualizado: FullPond = {nivel}");
                    }

                    // Verificar alertas
                    await _lowTemperatureAlert.Verificar(temperatura);
                    await _highTemperatureAlert.Verificar(temperatura);
                    await _waterLevelAlert.Verificar(nivel);
                    await _disconnectionAlert.Verificar();
                }
                else
                {
                    Console.WriteLine("Error: Datos incompletos o formato incorrecto.");
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error: Formato de datos incorrecto. {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
            }
        }

    }
}
