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
    public class CancelBookingtbsController : ControllerBase
    {
        private readonly hotelhubContext _context;

        public CancelBookingtbsController(hotelhubContext context)
        {
            _context = context;
        }

        // GET: api/CancelBookingtbs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CancelBookingtb>>> GetCancelBookingtbs()
        {
          if (_context.CancelBookingtbs == null)
          {
              return NotFound();
          }
            return await _context.CancelBookingtbs.ToListAsync();
        }

        // GET: api/CancelBookingtbs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CancelBookingtb>> GetCancelBookingtb(int id)
        {
          if (_context.CancelBookingtbs == null)
          {
              return NotFound();
          }
            var cancelBookingtb = await _context.CancelBookingtbs.FindAsync(id);

            if (cancelBookingtb == null)
            {
                return NotFound();
            }

            return cancelBookingtb;
        }

        // PUT: api/CancelBookingtbs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCancelBookingtb(int id, CancelBookingtb cancelBookingtb)
        {
            if (id != cancelBookingtb.Id)
            {
                return BadRequest();
            }

            _context.Entry(cancelBookingtb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CancelBookingtbExists(id))
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

        // POST: api/CancelBookingtbs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CancelBookingtb>> PostCancelBookingtb(CancelBookingtb cancelBookingtb)
        {
          if (_context.CancelBookingtbs == null)
          {
              return Problem("Entity set 'hotelhubContext.CancelBookingtbs'  is null.");
          }
            _context.CancelBookingtbs.Add(cancelBookingtb);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCancelBookingtb", new { id = cancelBookingtb.Id }, cancelBookingtb);
        }

        // DELETE: api/CancelBookingtbs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCancelBookingtb(int id)
        {
            if (_context.CancelBookingtbs == null)
            {
                return NotFound();
            }
            var cancelBookingtb = await _context.CancelBookingtbs.FindAsync(id);
            if (cancelBookingtb == null)
            {
                return NotFound();
            }

            _context.CancelBookingtbs.Remove(cancelBookingtb);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CancelBookingtbExists(int id)
        {
            return (_context.CancelBookingtbs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
