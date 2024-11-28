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
    public class RoomtbsController : ControllerBase
    {
        private readonly hotelhubContext _context;

        public RoomtbsController(hotelhubContext context)
        {
            _context = context;
        }

        // GET: api/Roomtbs
        [HttpGet("allrooms")]
        public async Task<ActionResult<IEnumerable<Roomtb>>> Getallrooms()
        {
          if (_context.Roomtbs == null)
          {
              return NotFound();
          }
            return await _context.Roomtbs.ToListAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Roomtb>>> GetRoomtbs()
        {
            if (_context.Roomtbs == null)
            {
                return NotFound();
            }

            // Get rooms and their associated category name
            var rooms = await (from room in _context.Roomtbs
                               where room.Hid == 1
                               select new
                               {
                                   room.Id,
                                   room.Roomcategoryid,
                                   CategoryName = room.Roomcategory.CategoryName, // Access the Name from the RoomCategorytb entity
                                   room.AdultCapacity,
                                   room.ChildrenCapacity,
                                   room.Quantity,
                                   room.Rent,
                                   room.Discount
                               }).ToListAsync();

            return Ok(rooms);
        }


        // GET: api/Roomtbs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Roomtb>> GetRoomtb(int id)
        {
          if (_context.Roomtbs == null)
          {
              return NotFound();
          }
            var roomtb = await _context.Roomtbs.FindAsync(id);

            if (roomtb == null)
            {
                return NotFound();
            }

            return roomtb;
        }

        // PUT: api/Roomtbs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoomtb(int id, Roomtb roomtb)
        {
            if (id != roomtb.Id)
            {
                return BadRequest();
            }

            _context.Entry(roomtb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomtbExists(id))
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

        // POST: api/Roomtbs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Roomtb>> PostRoomtb(Roomtb roomtb)
        {
            if (_context.Roomtbs == null)
                return Problem("Entity set 'hotelhubContext.Roomtbs' is null.");

            //var categorytb = await _context.RoomCategorytbs.FirstOrDefaultAsync(x => x.Id == roomtb.Roomcategoryid);
            //var hoteltb = await _context.Hoteltbs.FirstOrDefaultAsync(x => x.Id == roomtb.Hid);
            //var festivalDiscount = await _context.FestivalDiscounts.FirstOrDefaultAsync(x => x.Id == roomtb.FestivalId);

            //roomtb.Roomcategory = categorytb;
            //roomtb.HidNavigation = hoteltb;
            //roomtb.Festival = festivalDiscount;

            _context.Roomtbs.Add(roomtb);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRoomtb", new { id = roomtb.Id }, roomtb);
        }

        // DELETE: api/Roomtbs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoomtb(int id)
        {
            if (_context.Roomtbs == null)
            {
                return NotFound();
            }
            var roomtb = await _context.Roomtbs.FindAsync(id);
            if (roomtb == null)
            {
                return NotFound();
            }

            _context.Roomtbs.Remove(roomtb);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoomtbExists(int id)
        {
            return (_context.Roomtbs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
