using Aquarius.Data.Repositories;
using Aquarius.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Aquarius.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorsController : ControllerBase
    {
        private readonly ISensorRepository _sensorRepository;

        public SensorsController(ISensorRepository sensorRepository)
        {
            _sensorRepository = sensorRepository;
        }

        // GET: api/Sensors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sensor>>> GetSensors()
        {
            return Ok(await _sensorRepository.GetAllAsync());
        }

        // GET: api/Sensors/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Sensor>> GetSensor(Guid id)
        {
            var sensor = await _sensorRepository.GetByIdAsync(id);
            if (sensor == null)
            {
                return NotFound();
            }
            return Ok(sensor);
        }

        // POST: api/Sensors
        [HttpPost]
        public async Task<ActionResult<Sensor>> CreateSensor([FromBody] Sensor sensor)
        {
            if (sensor == null)
            {
                return BadRequest();
            }

            await _sensorRepository.AddAsync(sensor);
            return CreatedAtAction(nameof(GetSensor), new { id = sensor.Id }, sensor);
        }

        // PUT: api/Sensors/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSensor(Guid id, [FromBody] Sensor sensor)
        {
            if (id != sensor.Id)
            {
                return BadRequest();
            }

            await _sensorRepository.UpdateAsync(sensor);
            return NoContent();
        }

        // DELETE: api/Sensors/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSensor(Guid id)
        {
            await _sensorRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}