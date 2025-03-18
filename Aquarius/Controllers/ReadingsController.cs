using Aquarius.Data.Repositories;
using Aquarius.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Aquarius.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReadingsController : ControllerBase
    {
        private readonly IReadingRepository _readingRepository;

        public ReadingsController(IReadingRepository readingRepository)
        {
            _readingRepository = readingRepository;
        }

        // GET: api/Readings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reading>>> GetReadings()
        {
            return Ok(await _readingRepository.GetAllAsync());
        }

        // GET: api/Readings/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Reading>> GetReading(Guid id)
        {
            var reading = await _readingRepository.GetByIdAsync(id);
            if (reading == null)
            {
                return NotFound();
            }
            return Ok(reading);
        }

        // POST: api/Readings
        [HttpPost]
        public async Task<ActionResult<Reading>> CreateReading([FromBody] Reading reading)
        {
            if (reading == null)
            {
                return BadRequest();
            }

            await _readingRepository.AddAsync(reading);
            return CreatedAtAction(nameof(GetReading), new { id = reading.Id }, reading);
        }

        // PUT: api/Readings/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReading(Guid id, [FromBody] Reading reading)
        {
            if (id != reading.Id)
            {
                return BadRequest();
            }

            await _readingRepository.UpdateAsync(reading);
            return NoContent();
        }

        // DELETE: api/Readings/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReading(Guid id)
        {
            await _readingRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}