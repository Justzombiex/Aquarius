using Aquarius.Domain;

namespace Aquarius.Data.Repositories
{
    public interface IPondRepository
    {
        Task<Pond> GetByIdAsync(Guid id);
        Task<IEnumerable<Pond>> GetAllAsync();
        Task AddAsync(Pond pond);
        Task UpdateAsync(Pond pond);
        Task DeleteAsync(Guid id);
    }
}