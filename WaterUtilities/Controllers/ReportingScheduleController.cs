using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WaterUtilities.Data;
using WaterUtilities.Models;

namespace WaterUtilities.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportingScheduleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReportingScheduleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ReportingSchedule
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReportingSchedule>>> GetReportingSchedules()
        {
            return await _context.ReportingSchedules.ToListAsync();
        }

        // GET: api/ReportingSchedule/5
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ReportingSchedule>> GetReportingSchedule(Guid id)
        {
            var reportingSchedule = await _context.ReportingSchedules.FindAsync(id);

            if (reportingSchedule == null)
            {
                return NotFound();
            }

            return reportingSchedule;
        }

        // PUT: api/ReportingSchedule/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReportingSchedule(Guid id, ReportingSchedule reportingSchedule)
        {
            if (id != reportingSchedule.Id)
            {
                return BadRequest();
            }

            _context.Entry(reportingSchedule).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReportingScheduleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ReportingSchedule
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ReportingSchedule>> PostReportingSchedule(ReportingSchedule reportingSchedule)
        {
            _context.ReportingSchedules.Add(reportingSchedule);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReportingSchedule", new { id = reportingSchedule.Id }, reportingSchedule);
        }

        // DELETE: api/ReportingSchedule/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReportingSchedule(Guid id)
        {
            var reportingSchedule = await _context.ReportingSchedules.FindAsync(id);
            if (reportingSchedule == null)
            {
                return NotFound();
            }

            _context.ReportingSchedules.Remove(reportingSchedule);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReportingScheduleExists(Guid id)
        {
            return _context.ReportingSchedules.Any(e => e.Id == id);
        }
    }
}
