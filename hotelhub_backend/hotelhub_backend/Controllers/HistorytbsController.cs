using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hotelhub_backend.Models;

namespace hotelhub_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistorytbsController : ControllerBase
    {
        private readonly hotelhubContext _context;

        public HistorytbsController(hotelhubContext context)
        {
            _context = context;
        }

        // GET: api/Historytbs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Historytb>>> GetHistorytbs()
        {
          if (_context.Historytbs == null)
          {
              return NotFound();
          }
            return await _context.Historytbs.ToListAsync();
        }

        // GET: api/Historytbs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Historytb>> GetHistorytb(int id)
        {
          if (_context.Historytbs == null)
          {
              return NotFound();
          }
            var historytb = await _context.Historytbs.FindAsync(id);

            if (historytb == null)
            {
                return NotFound();
            }

            return historytb;
        }

        // PUT: api/Historytbs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHistorytb(int id, Historytb historytb)
        {
            if (id != historytb.Id)
            {
                return BadRequest();
            }

            _context.Entry(historytb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HistorytbExists(id))
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

        // POST: api/Historytbs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Historytb>> PostHistorytb(Historytb historytb)
        {
          if (_context.Historytbs == null)
          {
              return Problem("Entity set 'hotelhubContext.Historytbs'  is null.");
          }
            //historytb.CheckIn = DateTime.Now.Date; // Only the date part will be stored
            //historytb.CheckOut = DateTime.Now.Date;
            if (historytb.CheckIn == default || historytb.CheckOut == default)
            {
                return BadRequest("CheckIn and CheckOut dates must be provided.");
            }
            _context.Historytbs.Add(historytb);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHistorytb", new { id = historytb.Id }, historytb);
        }

        // DELETE: api/Historytbs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHistorytb(int id)
        {
            if (_context.Historytbs == null)
            {
                return NotFound();
            }
            var historytb = await _context.Historytbs.FindAsync(id);
            if (historytb == null)
            {
                return NotFound();
            }

            _context.Historytbs.Remove(historytb);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HistorytbExists(int id)
        {
            return (_context.Historytbs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
