using Aquarius.Domain;

namespace Aquarius.Data.Repositories
{
    public interface ITemperatureSensorRepository
    {
        Task<TemperatureSensor> GetByIdAsync(Guid id);
        Task<IEnumerable<TemperatureSensor>> GetAllAsync();
        Task AddAsync(TemperatureSensor sensor);
        Task UpdateAsync(TemperatureSensor sensor);
        Task DeleteAsync(Guid id);
    }
}