using Aquarius.Data.Repositories;
using Aquarius.Domain;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Aquarius.Services.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowAngularApp")]
    public class AlertsController : ControllerBase
    {
        private readonly IAlertRepository _AlertRepository;

        public AlertsController(IAlertRepository AlertRepository)
        {
            _AlertRepository = AlertRepository;
        }

        // GET: api/Alerts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Alert>>> GetAlerts()
        {
            return Ok(await _AlertRepository.GetAllAsync());
        }

        // GET: api/Alerts/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Alert>> GetAlert(Guid id)
        {
            var Alert = await _AlertRepository.GetByIdAsync(id);
            if (Alert == null)
            {
                return NotFound();
            }
            return Ok(Alert);
        }

        // POST: api/Alerts
        [HttpPost]
        public async Task<ActionResult<Alert>> CreateAlert([FromBody] Alert Alert)
        {
            if (Alert == null)
            {
                return BadRequest();
            }

            await _AlertRepository.AddAsync(Alert);
            return CreatedAtAction(nameof(GetAlert), new { id = Alert.Id }, Alert);
        }

        // PUT: api/Alerts/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAlert(Guid id, [FromBody] Alert Alert)
        {
            if (id != Alert.Id)
            {
                return BadRequest();
            }

            await _AlertRepository.UpdateAsync(Alert);
            return NoContent();
        }

        // DELETE: api/Alerts/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlert(Guid id)
        {
            await _AlertRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
