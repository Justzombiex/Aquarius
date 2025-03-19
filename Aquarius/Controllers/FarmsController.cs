using Aquarius.Data.Repositories;
using Aquarius.Domain;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Aquarius.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowAngularApp")]
    public class FarmsController : ControllerBase
    {
        private readonly IFarmRepository _farmRepository;

        public FarmsController(IFarmRepository farmRepository)
        {
            _farmRepository = farmRepository;
        }

        // GET: api/Farms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Farm>>> GetFarms()
        {
            return Ok(await _farmRepository.GetAllAsync());
        }

        // GET: api/Farms/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Farm>> GetFarm(Guid id)
        {
            var farm = await _farmRepository.GetByIdAsync(id);
            if (farm == null)
            {
                return NotFound();
            }
            return Ok(farm);
        }

        // POST: api/Farms
        [HttpPost]
        public async Task<ActionResult<Farm>> CreateFarm([FromBody] Farm farm)
        {
            if (farm == null)
            {
                return BadRequest();
            }

            await _farmRepository.AddAsync(farm);
            return CreatedAtAction(nameof(GetFarm), new { id = farm.Id }, farm);
        }

        // PUT: api/Farms/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFarm(Guid id, [FromBody] Farm farm)
        {
            if (id != farm.Id)
            {
                return BadRequest();
            }

            await _farmRepository.UpdateAsync(farm);
            return NoContent();
        }

        // DELETE: api/Farms/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFarm(Guid id)
        {
            await _farmRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}