using Aquarius.Domain;

namespace Aquarius.Data.Repositories
{
    public interface ISensorRepository
    {
        Task<Sensor> GetByIdAsync(Guid id);
        Task<IEnumerable<Sensor>> GetAllAsync();
        Task AddAsync(Sensor sensor);
        Task UpdateAsync(Sensor sensor);
        Task DeleteAsync(Guid id);
    }
}