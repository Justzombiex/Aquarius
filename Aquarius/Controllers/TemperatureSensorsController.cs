using Aquarius.Data.Repositories;
using Aquarius.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Aquarius.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemperatureSensorsController : ControllerBase
    {
        private readonly ITemperatureSensorRepository _temperatureSensorRepository;

        public TemperatureSensorsController(ITemperatureSensorRepository temperaturesensorRepository)
        {
            _temperatureSensorRepository = temperaturesensorRepository;
        }

        // GET: api/TemperatureSensors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TemperatureSensor>>> GetTemperatureSensors()
        {
            return Ok(await _temperatureSensorRepository.GetAllAsync());
        }

        // GET: api/TemperatureSensors/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TemperatureSensor>> GetTemperatureSensor(Guid id)
        {
            var temperaturesensor = await _temperatureSensorRepository.GetByIdAsync(id);
            if (temperaturesensor == null)
            {
                return NotFound();
            }
            return Ok(temperaturesensor);
        }

        // POST: api/TemperatureSensors
        [HttpPost]
        public async Task<ActionResult<TemperatureSensor>> CreateTemperatureSensor([FromBody] TemperatureSensor temperaturesensor)
        {
            if (temperaturesensor == null)
            {
                return BadRequest();
            }

            await _temperatureSensorRepository.AddAsync(temperaturesensor);
            return CreatedAtAction(nameof(GetTemperatureSensor), new { id = temperaturesensor.Id }, temperaturesensor);
        }

        // PUT: api/TemperatureSensors/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTemperatureSensor(Guid id, [FromBody] TemperatureSensor temperaturesensor)
        {
            if (id != temperaturesensor.Id)
            {
                return BadRequest();
            }

            await _temperatureSensorRepository.UpdateAsync(temperaturesensor);
            return NoContent();
        }

        // DELETE: api/TemperatureSensors/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemperatureSensor(Guid id)
        {
            await _temperatureSensorRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}