using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hotelhub_backend.Models;
using Org.BouncyCastle.Utilities;

namespace hotelhub_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomCategorytbsController : ControllerBase
    {
        private readonly hotelhubContext _context;

        public RoomCategorytbsController(hotelhubContext context)
        {
            _context = context;
        }

        // GET: api/RoomCategorytbs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomCategorytb>>> GetRoomCategorytbs()
        {
          if (_context.RoomCategorytbs == null)
          {
              return NotFound();
          }
            return await _context.RoomCategorytbs.ToListAsync();
        }

        [HttpGet("getcategorycount")]
        public async Task<int> getcategorycount()
        {
            int catcount = await _context.RoomCategorytbs.CountAsync();
            return catcount;
        }

        // GET: api/RoomCategorytbs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RoomCategorytb>> GetRoomCategorytb(int id)
        {
          if (_context.RoomCategorytbs == null)
          {
              return NotFound();
          }
            var roomCategorytb = await _context.RoomCategorytbs.FindAsync(id);

            if (roomCategorytb == null)
            {
                return NotFound();
            }

            return roomCategorytb;
        }

        // PUT: api/RoomCategorytbs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoomCategorytb(int id, RoomCategorytb roomCategorytb)
        {
            if (id != roomCategorytb.Id)
            {
                return BadRequest();
            }

            _context.Entry(roomCategorytb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomCategorytbExists(id))
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

        // POST: api/RoomCategorytbs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RoomCategorytb>> PostRoomCategorytb(RoomCategorytb roomCategorytb)
        {
          if (_context.RoomCategorytbs == null)
          {
              return Problem("Entity set 'hotelhubContext.RoomCategorytbs'  is null.");
          }
            _context.RoomCategorytbs.Add(roomCategorytb);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRoomCategorytb", new { id = roomCategorytb.Id }, roomCategorytb);
        }

        // DELETE: api/RoomCategorytbs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoomCategorytb(int id)
        {
            if (_context.RoomCategorytbs == null)
            {
                return NotFound();
            }
            var roomCategorytb = await _context.RoomCategorytbs.FindAsync(id);
            if (roomCategorytb == null)
            {
                return NotFound();
            }

            _context.RoomCategorytbs.Remove(roomCategorytb);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoomCategorytbExists(int id)
        {
            return (_context.RoomCategorytbs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
