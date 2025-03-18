using Aquarius.Data.Repositories;
using Aquarius.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Aquarius.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PondsController : ControllerBase
    {
        private readonly IPondRepository _pondRepository;

        public PondsController(IPondRepository pondRepository)
        {
            _pondRepository = pondRepository;
        }

        // GET: api/Ponds
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pond>>> GetPonds()
        {
            return Ok(await _pondRepository.GetAllAsync());
        }

        // GET: api/Ponds/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Pond>> GetPond(Guid id)
        {
            var pond = await _pondRepository.GetByIdAsync(id);
            if (pond == null)
            {
                return NotFound();
            }
            return Ok(pond);
        }

        // POST: api/Ponds
        [HttpPost]
        public async Task<ActionResult<Pond>> CreatePond([FromBody] Pond pond)
        {
            if (pond == null)
            {
                return BadRequest();
            }

            await _pondRepository.AddAsync(pond);
            return CreatedAtAction(nameof(GetPond), new { id = pond.Id }, pond);
        }

        // PUT: api/Ponds/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePond(Guid id, [FromBody] Pond pond)
        {
            if (id != pond.Id)
            {
                return BadRequest();
            }

            await _pondRepository.UpdateAsync(pond);
            return NoContent();
        }

        // DELETE: api/Ponds/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePond(Guid id)
        {
            await _pondRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}