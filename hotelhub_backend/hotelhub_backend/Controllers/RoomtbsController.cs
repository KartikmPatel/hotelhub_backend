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

        [HttpGet("getroomcount/{hid}")]
        public async Task<int> getroomcount(int hid)
        {
            int roomCount = await _context.Roomtbs.CountAsync(r => r.Hid == hid);
            return roomCount;
        }

        [HttpGet("hotel/{hid}")]
        public async Task<ActionResult<IEnumerable<object>>> GetRoomtbsByHotelId(int hid)
        {
            if (_context.Roomtbs == null)
            {
                return NotFound();
            }

            var rooms = await (from room in _context.Roomtbs
                               where room.Hid == hid
                               select new
                               {
                                   room.Id,
                                   room.Roomcategoryid,
                                   CategoryName = room.Roomcategory.CategoryName,
                                   room.AdultCapacity,
                                   room.ChildrenCapacity,
                                   room.Quantity,
                                   room.City,
                                   room.Rent,
                                   room.Discount,
                                   room.ActiveStatus
                               }).ToListAsync();

            if (rooms.Count == 0)
            {
                return NotFound("No rooms found for the given hotel ID.");
            }

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
        public async Task<IActionResult> PutRoomtb(int id, [FromBody] Roomtb roomtb)
        {
            if (id != roomtb.Id)
            {
                return BadRequest();
            }

            // Check if the room with the same 'Hid', 'Roomcategoryid', and 'City' already exists in the database
            var existingRoomWithSameDetails = await _context.Roomtbs
                .FirstOrDefaultAsync(r => r.Hid == roomtb.Hid && r.Roomcategoryid == roomtb.Roomcategoryid && r.City == roomtb.City && r.Id != id);

            if (existingRoomWithSameDetails != null)
            {
                return Conflict("A room with the same Category, and City already exists.");
            }

            // Ensure the room exists before updating
            var existingRoom = await _context.Roomtbs.FindAsync(id);
            if (existingRoom == null)
            {
                return NotFound();
            }

            // Update fields individually to avoid overwriting unintended data
            existingRoom.Roomcategoryid = roomtb.Roomcategoryid;
            existingRoom.AdultCapacity = roomtb.AdultCapacity;
            existingRoom.ChildrenCapacity = roomtb.ChildrenCapacity;
            existingRoom.Quantity = roomtb.Quantity;
            existingRoom.City = roomtb.City;
            existingRoom.Rent = roomtb.Rent;
            existingRoom.Discount = roomtb.Discount;
            existingRoom.ActiveStatus = roomtb.ActiveStatus;
            existingRoom.FestivalId = roomtb.FestivalId;
            existingRoom.Hid = roomtb.Hid;

            _context.Entry(existingRoom).State = EntityState.Modified;

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

            // Check if a room with the same Hid, Roomcategoryid, and City already exists
            var existingRoom = await _context.Roomtbs
                .FirstOrDefaultAsync(r => r.Hid == roomtb.Hid &&
                                          r.Roomcategoryid == roomtb.Roomcategoryid &&
                                          r.City == roomtb.City);

            if (existingRoom != null)
            {
                return BadRequest(new { message = "Room category in this city already exists." });
            }

            // Add the new room if validation passes
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

        [HttpGet("changeActiveStatus/{status}")]
        public async Task<IActionResult> changeActiveStatus(int roomid,int status)
        {
            var room = await _context.Roomtbs.FindAsync(roomid);
            if (room == null)
            {
                return NotFound();
            }

            if(status == 0)
            {
                room.ActiveStatus = 1;
            }
            else
            {
                room.ActiveStatus = 0;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("check-existence/{roomId}")]
        public async Task<IActionResult> CheckRoomExistence(int roomId, [FromQuery] string city, [FromQuery] int categoryId, [FromQuery] int hid)
        {
            var roomExists = await _context.Roomtbs
                .AnyAsync(r => r.Hid == hid && r.Roomcategoryid == categoryId && r.City == city && r.Id != roomId);

            return Ok(new { exists = roomExists });
        }
    }

}
