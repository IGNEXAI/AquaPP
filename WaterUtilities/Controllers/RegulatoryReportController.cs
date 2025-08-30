using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WaterUtilities.Data;
using WaterUtilities.Models;

namespace WaterUtilities.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegulatoryReportController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RegulatoryReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/RegulatoryReport
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RegulatoryReport>>> GetRegulatoryReports()
        {
            var reports = await _context.RegulatoryReports.ToListAsync();
            return Ok(reports); 
        }

        // GET: api/RegulatoryReport/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RegulatoryReport>> GetRegulatoryReport(Guid id)
        {
            var regulatoryReport = await _context.RegulatoryReports.FindAsync(id);

            if (regulatoryReport == null)
            {
                return NotFound();
            }

            return Ok(regulatoryReport);
        }

        // PUT: api/RegulatoryReport/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> PutRegulatoryReport(Guid id, RegulatoryReport regulatoryReport)
        {
            if (id != regulatoryReport.Id)
            {
                return BadRequest();
            }

            _context.Entry(regulatoryReport).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RegulatoryReportExists(id))
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

        // POST: api/RegulatoryReport
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RegulatoryReport>> PostRegulatoryReport(RegulatoryReport regulatoryReport)
        {
            _context.RegulatoryReports.Add(regulatoryReport);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRegulatoryReport", new { id = regulatoryReport.Id }, regulatoryReport);
        }

        // DELETE: api/RegulatoryReport/5
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteRegulatoryReport(Guid id)
        {
            var regulatoryReport = await _context.RegulatoryReports.FindAsync(id);
            if (regulatoryReport == null)
            {
                return NotFound();
            }

            _context.RegulatoryReports.Remove(regulatoryReport);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RegulatoryReportExists(Guid id)
        {
            return _context.RegulatoryReports.Any(e => e.Id == id);
        }
    }
}
