using Aquarius.Domain;
using Microsoft.EntityFrameworkCore;

namespace Aquarius.Data.Repositories
{
    public class SensorRepository : ISensorRepository
    {
        private readonly AquariusDbContext _context;

        public SensorRepository(AquariusDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Sensor> GetByIdAsync(Guid id)
        {
            return await _context.Sensors.FindAsync(id);
        }

        public async Task<IEnumerable<Sensor>> GetAllAsync()
        {
            return await _context.Sensors.ToListAsync();
        }

        public async Task AddAsync(Sensor sensor)
        {
            await _context.Sensors.AddAsync(sensor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Sensor sensor)
        {
            _context.Sensors.Update(sensor);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var sensor = await _context.Sensors.FindAsync(id);
            if (sensor != null)
            {
                _context.Sensors.Remove(sensor);
                await _context.SaveChangesAsync();
            }
        }
    }
}