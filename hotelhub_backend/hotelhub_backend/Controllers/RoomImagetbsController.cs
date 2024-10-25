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
    public class RoomImagetbsController : ControllerBase
    {
        private readonly hotelhubContext _context;

        public RoomImagetbsController(hotelhubContext context)
        {
            _context = context;
        }

        // GET: api/RoomImagetbs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomImagetb>>> GetRoomImagetbs()
        {
          if (_context.RoomImagetbs == null)
          {
              return NotFound();
          }
            return await _context.RoomImagetbs.ToListAsync();
        }

        // GET: api/RoomImagetbs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RoomImagetb>> GetRoomImagetb(int id)
        {
          if (_context.RoomImagetbs == null)
          {
              return NotFound();
          }
            var roomImagetb = await _context.RoomImagetbs.FindAsync(id);

            if (roomImagetb == null)
            {
                return NotFound();
            }

            return roomImagetb;
        }

        // PUT: api/RoomImagetbs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoomImagetb(int id, RoomImagetb roomImagetb)
        {
            if (id != roomImagetb.Id)
            {
                return BadRequest();
            }

            _context.Entry(roomImagetb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomImagetbExists(id))
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

        // POST: api/RoomImagetbs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RoomImagetb>> PostRoomImagetb(RoomImagetb roomImagetb)
        {
          if (_context.RoomImagetbs == null)
          {
              return Problem("Entity set 'hotelhubContext.RoomImagetbs'  is null.");
          }
            _context.RoomImagetbs.Add(roomImagetb);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRoomImagetb", new { id = roomImagetb.Id }, roomImagetb);
        }

        // DELETE: api/RoomImagetbs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoomImagetb(int id)
        {
            if (_context.RoomImagetbs == null)
            {
                return NotFound();
            }
            var roomImagetb = await _context.RoomImagetbs.FindAsync(id);
            if (roomImagetb == null)
            {
                return NotFound();
            }

            _context.RoomImagetbs.Remove(roomImagetb);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoomImagetbExists(int id)
        {
            return (_context.RoomImagetbs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
