using Aquarius.Domain;

namespace Aquarius.Data.Repositories
{
    public interface ILevelSensorRepository
    {
        Task<LevelSensor> GetByIdAsync(Guid id); // Obtener un sensor de nivel por ID
        Task<IEnumerable<LevelSensor>> GetAllAsync(); // Obtener todos los sensores de nivel
        Task AddAsync(LevelSensor sensorLevel); // Agregar un nuevo sensor de nivel
        Task UpdateAsync(LevelSensor sensorLevel); // Actualizar un sensor de nivel existente
        Task DeleteAsync(Guid id); // Eliminar un sensor de nivel por ID
    }
}
