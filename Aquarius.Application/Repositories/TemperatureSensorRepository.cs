using Aquarius.Domain;
using Microsoft.EntityFrameworkCore;

namespace Aquarius.Data.Repositories
{
    public class TemperatureSensorRepository : ITemperatureSensorRepository
    {
        private readonly AquariusDbContext _context;

        public TemperatureSensorRepository(AquariusDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<TemperatureSensor> GetByIdAsync(Guid id)
        {
            return await _context.TemperatureSensors.FindAsync(id);
        }

        public async Task<IEnumerable<TemperatureSensor>> GetAllAsync()
        {
            return await _context.TemperatureSensors.ToListAsync();
        }

        public async Task AddAsync(TemperatureSensor temperaturesensor)
        {
            await _context.TemperatureSensors.AddAsync(temperaturesensor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TemperatureSensor temperaturesensor)
        {
            _context.TemperatureSensors.Update(temperaturesensor);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var temperaturesensor = await _context.TemperatureSensors.FindAsync(id);
            if (temperaturesensor != null)
            {
                _context.TemperatureSensors.Remove(temperaturesensor);
                await _context.SaveChangesAsync();
            }
        }
    }
}