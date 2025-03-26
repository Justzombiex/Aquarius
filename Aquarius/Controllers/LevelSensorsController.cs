using Aquarius.Data.Repositories;
using Aquarius.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Aquarius.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LevelSensorsController : ControllerBase
    {
        private readonly ILevelSensorRepository _levelSensorRepository;

        public LevelSensorsController(ILevelSensorRepository levelSensorRepository)
        {
            _levelSensorRepository = levelSensorRepository;
        }

        // GET: api/LevelSensors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LevelSensor>>> GetLevelSensors()
        {
            return Ok(await _levelSensorRepository.GetAllAsync());
        }

        // GET: api/LevelSensors/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<LevelSensor>> GetLevelSensor(Guid id)
        {
            var levelSensor = await _levelSensorRepository.GetByIdAsync(id);
            if (levelSensor == null)
            {
                return NotFound();
            }
            return Ok(levelSensor);
        }

        // POST: api/LevelSensors
        [HttpPost]
        public async Task<ActionResult<LevelSensor>> CreateLevelSensor([FromBody] LevelSensor levelSensor)
        {
            if (levelSensor == null)
            {
                return BadRequest();
            }

            await _levelSensorRepository.AddAsync(levelSensor);
            return CreatedAtAction(nameof(GetLevelSensor), new { id = levelSensor.Id }, levelSensor);
        }

        // PUT: api/LevelSensors/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLevelSensor(Guid id, [FromBody] LevelSensor levelSensor)
        {
            if (id != levelSensor.Id)
            {
                return BadRequest();
            }

            await _levelSensorRepository.UpdateAsync(levelSensor);
            return NoContent();
        }

        // DELETE: api/LevelSensors/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLevelSensor(Guid id)
        {
            await _levelSensorRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
