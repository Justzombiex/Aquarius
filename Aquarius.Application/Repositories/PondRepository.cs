using Aquarius.Domain;
using Microsoft.EntityFrameworkCore;

namespace Aquarius.Data.Repositories
{
    public class PondRepository : IPondRepository
    {
        private readonly AquariusDbContext _context;

        public PondRepository(AquariusDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Pond> GetByIdAsync(Guid id)
        {
            return await _context.Ponds.FindAsync(id);
        }

        public async Task<IEnumerable<Pond>> GetAllAsync()
        {
            return await _context.Ponds.ToListAsync();
        }

        public async Task AddAsync(Pond pond)
        {
            await _context.Ponds.AddAsync(pond);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Pond pond)
        {
            _context.Ponds.Update(pond);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var pond = await _context.Ponds.FindAsync(id);
            if (pond != null)
            {
                _context.Ponds.Remove(pond);
                await _context.SaveChangesAsync();
            }
        }
    }
}