using Aquarius.Domain;

namespace Aquarius.Data.Repositories
{
    public interface IAlertRepository
    {
        Task<Alert> GetByIdAsync(Guid id);
        Task<IEnumerable<Alert>> GetAllAsync();
        Task AddAsync(Alert alert);
        Task UpdateAsync(Alert alert);
        Task DeleteAsync(Guid id);
    }
}