using Aquarius.Domain;

namespace Aquarius.Data.Repositories
{
    public interface IReadingRepository
    {
        Task<Reading> GetByIdAsync(Guid id);
        Task<IEnumerable<Reading>> GetAllAsync();
        Task AddAsync(Reading reading);
        Task UpdateAsync(Reading reading);
        Task DeleteAsync(Guid id);
    }
}