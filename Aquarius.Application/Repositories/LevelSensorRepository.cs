using Aquarius.Domain;
using Microsoft.EntityFrameworkCore;

namespace Aquarius.Data.Repositories
{
    public class LevelSensorRepository : ILevelSensorRepository
    {
        private readonly AquariusDbContext _context;

        public LevelSensorRepository(AquariusDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<LevelSensor> GetByIdAsync(Guid id)
        {
            return await _context.LevelSensors.FindAsync(id);
        }

        public async Task<IEnumerable<LevelSensor>> GetAllAsync()
        {
            return await _context.LevelSensors.ToListAsync();
        }

        public async Task AddAsync(LevelSensor levelSensor)
        {
            await _context.LevelSensors.AddAsync(levelSensor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(LevelSensor levelSensor)
        {
            _context.LevelSensors.Update(levelSensor);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var levelSensor = await _context.LevelSensors.FindAsync(id);
            if (levelSensor != null)
            {
                _context.LevelSensors.Remove(levelSensor);
                await _context.SaveChangesAsync();
            }
        }
    }
}
