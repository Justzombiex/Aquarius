using Aquarius.Data.Repositories;
using Aquarius.Domain;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Aquarius.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowAngularApp")]
    public class TemperatureSensorsController : ControllerBase
    {
        private readonly ITemperatureSensorRepository _temperatureSensorRepository;

        public TemperatureSensorsController(ITemperatureSensorRepository temperatureSensorRepository)
        {
            _temperatureSensorRepository = temperatureSensorRepository;
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
            var temperatureSensor = await _temperatureSensorRepository.GetByIdAsync(id);
            if (temperatureSensor == null)
            {
                return NotFound();
            }
            return Ok(temperatureSensor);
        }

        // POST: api/TemperatureSensors
        [HttpPost]
        public async Task<ActionResult<TemperatureSensor>> CreateTemperatureSensor([FromBody] TemperatureSensor temperatureSensor)
        {
            if (temperatureSensor == null)
            {
                return BadRequest();
            }

            await _temperatureSensorRepository.AddAsync(temperatureSensor);
            return CreatedAtAction(nameof(GetTemperatureSensor), new { id = temperatureSensor.Id }, temperatureSensor);
        }

        // PUT: api/TemperatureSensors/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTemperatureSensor(Guid id, [FromBody] TemperatureSensor temperatureSensor)
        {
            if (id != temperatureSensor.Id)
            {
                return BadRequest();
            }

            await _temperatureSensorRepository.UpdateAsync(temperatureSensor);
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
