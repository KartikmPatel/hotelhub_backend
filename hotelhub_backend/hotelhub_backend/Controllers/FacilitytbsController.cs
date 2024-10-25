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
    public class FacilitytbsController : ControllerBase
    {
        private readonly hotelhubContext _context;

        public FacilitytbsController(hotelhubContext context)
        {
            _context = context;
        }

        // GET: api/Facilitytbs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Facilitytb>>> GetFacilitytbs()
        {
          if (_context.Facilitytbs == null)
          {
              return NotFound();
          }
            return await _context.Facilitytbs.ToListAsync();
        }

        // GET: api/Facilitytbs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Facilitytb>> GetFacilitytb(int id)
        {
          if (_context.Facilitytbs == null)
          {
              return NotFound();
          }
            var facilitytb = await _context.Facilitytbs.FindAsync(id);

            if (facilitytb == null)
            {
                return NotFound();
            }

            return facilitytb;
        }

        // PUT: api/Facilitytbs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFacilitytb(int id, Facilitytb facilitytb)
        {
            if (id != facilitytb.Id)
            {
                return BadRequest();
            }

            _context.Entry(facilitytb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FacilitytbExists(id))
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

        // POST: api/Facilitytbs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Facilitytb>> PostFacilitytb(Facilitytb facilitytb)
        {
          if (_context.Facilitytbs == null)
          {
              return Problem("Entity set 'hotelhubContext.Facilitytbs'  is null.");
          }
            _context.Facilitytbs.Add(facilitytb);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFacilitytb", new { id = facilitytb.Id }, facilitytb);
        }

        // DELETE: api/Facilitytbs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFacilitytb(int id)
        {
            if (_context.Facilitytbs == null)
            {
                return NotFound();
            }
            var facilitytb = await _context.Facilitytbs.FindAsync(id);
            if (facilitytb == null)
            {
                return NotFound();
            }

            _context.Facilitytbs.Remove(facilitytb);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FacilitytbExists(int id)
        {
            return (_context.Facilitytbs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
