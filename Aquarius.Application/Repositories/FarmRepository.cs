using Aquarius.Domain;
using Microsoft.EntityFrameworkCore;

namespace Aquarius.Data.Repositories
{
    public class FarmRepository : IFarmRepository
    {
        private readonly AquariusDbContext _context;

        public FarmRepository(AquariusDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Farm> GetByIdAsync(Guid id)
        {
            return await _context.Farms.FindAsync(id);
        }

        public async Task<IEnumerable<Farm>> GetAllAsync()
        {
            return await _context.Farms.ToListAsync();
        }

        public async Task AddAsync(Farm farm)
        {
            await _context.Farms.AddAsync(farm);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Farm farm)
        {
            _context.Farms.Update(farm);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var farm = await _context.Farms.FindAsync(id);
            if (farm != null)
            {
                _context.Farms.Remove(farm);
                await _context.SaveChangesAsync();
            }
        }
    }
}