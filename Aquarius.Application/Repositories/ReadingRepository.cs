using Aquarius.Domain;
using Microsoft.EntityFrameworkCore;

namespace Aquarius.Data.Repositories
{
    public class ReadingRepository : IReadingRepository
    {
        private readonly AquariusDbContext _context;

        public ReadingRepository(AquariusDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Reading> GetByIdAsync(Guid id)
        {
            return await _context.Readings.FindAsync(id);
        }

        public async Task<IEnumerable<Reading>> GetAllAsync()
        {
            return await _context.Readings.ToListAsync();
        }

        public async Task AddAsync(Reading reading)
        {
            await _context.Readings.AddAsync(reading);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Reading reading)
        {
            _context.Readings.Update(reading);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var reading = await _context.Readings.FindAsync(id);
            if (reading != null)
            {
                _context.Readings.Remove(reading);
                await _context.SaveChangesAsync();
            }
        }
    }
}