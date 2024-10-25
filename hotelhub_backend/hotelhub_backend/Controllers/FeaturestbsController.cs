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
    public class FeaturestbsController : ControllerBase
    {
        private readonly hotelhubContext _context;

        public FeaturestbsController(hotelhubContext context)
        {
            _context = context;
        }

        // GET: api/Featurestbs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Featurestb>>> GetFeaturestbs()
        {
          if (_context.Featurestbs == null)
          {
              return NotFound();
          }
            return await _context.Featurestbs.ToListAsync();
        }

        // GET: api/Featurestbs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Featurestb>> GetFeaturestb(int id)
        {
          if (_context.Featurestbs == null)
          {
              return NotFound();
          }
            var featurestb = await _context.Featurestbs.FindAsync(id);

            if (featurestb == null)
            {
                return NotFound();
            }

            return featurestb;
        }

        // PUT: api/Featurestbs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFeaturestb(int id, Featurestb featurestb)
        {
            if (id != featurestb.Id)
            {
                return BadRequest();
            }

            _context.Entry(featurestb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FeaturestbExists(id))
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

        // POST: api/Featurestbs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Featurestb>> PostFeaturestb(Featurestb featurestb)
        {
          if (_context.Featurestbs == null)
          {
              return Problem("Entity set 'hotelhubContext.Featurestbs'  is null.");
          }
            _context.Featurestbs.Add(featurestb);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFeaturestb", new { id = featurestb.Id }, featurestb);
        }

        // DELETE: api/Featurestbs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeaturestb(int id)
        {
            if (_context.Featurestbs == null)
            {
                return NotFound();
            }
            var featurestb = await _context.Featurestbs.FindAsync(id);
            if (featurestb == null)
            {
                return NotFound();
            }

            _context.Featurestbs.Remove(featurestb);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FeaturestbExists(int id)
        {
            return (_context.Featurestbs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
