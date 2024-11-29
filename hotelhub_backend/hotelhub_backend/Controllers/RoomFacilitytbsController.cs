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
    public class RoomFacilitytbsController : ControllerBase
    {
        private readonly hotelhubContext _context;

        public RoomFacilitytbsController(hotelhubContext context)
        {
            _context = context;
        }

        // GET: api/RoomFacilitytbs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomFacilitytb>>> GetRoomFacilitytbs(int roomid)
        {
            if (_context.RoomFacilitytbs == null)
            {
                return NotFound();
            }

            var roomFacilities = await (from facility in _context.RoomFacilitytbs
                                        where facility.RoomId == roomid
                                        select facility).ToListAsync();

            return Ok(roomFacilities);
        }


        // GET: api/RoomFacilitytbs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RoomFacilitytb>> GetRoomFacilitytb(int id)
        {
          if (_context.RoomFacilitytbs == null)
          {
              return NotFound();
          }
            var roomFacilitytb = await _context.RoomFacilitytbs.FindAsync(id);

            if (roomFacilitytb == null)
            {
                return NotFound();
            }

            return roomFacilitytb;
        }

        // GET: api/RoomFacilitytbs/getAllFacilityByHotel/5
        [HttpGet("getAllFacilityByHotel/{id}")]
        public async Task<ActionResult<IEnumerable<object>>> GetRoomFacilityByHotel(int id)
        {
            var facilities = await (from facility in _context.Facilitytbs
                                    join roomFacility in _context.RoomFacilitytbs on facility.Id equals roomFacility.FacilityId
                                    join room in _context.Roomtbs on roomFacility.RoomId equals room.Id
                                    where room.Hid == id
                                    select new
                                    {
                                        facility.FacilityName,
                                        facility.Image
                                    }).Distinct().ToListAsync();

            if (facilities == null)
            {
                return NotFound();
            }

            return facilities;
        }

        // PUT: api/RoomFacilitytbs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoomFacilitytb(int id, RoomFacilitytb roomFacilitytb)
        {
            if (id != roomFacilitytb.Id)
            {
                return BadRequest();
            }

            _context.Entry(roomFacilitytb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomFacilitytbExists(id))
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

        // POST: api/RoomFacilitytbs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RoomFacilitytb>> PostRoomFacilitytb([FromBody] RoomFacilitytb request)
        {
            if (_context.RoomFacilitytbs == null)
            {
                return Problem("Entity set 'hotelhubContext.RoomFacilitytbs' is null.");
            }

            var roomFacilitytb = new RoomFacilitytb
            {
                FacilityId = request.FacilityId,  
                RoomId = request.RoomId          
            };


            _context.RoomFacilitytbs.Add(roomFacilitytb);
            await _context.SaveChangesAsync();  

            
            return CreatedAtAction("GetRoomFacilitytbs", new { roomid = roomFacilitytb.RoomId }, roomFacilitytb);
        }



        
        [HttpDelete("{roomid}")]
        public async Task<IActionResult> DeleteRoomFacilitytb(int roomid)
        {
            if (_context.RoomFacilitytbs == null)
            {
                return NotFound();
            }

            
            var roomFacilities = await _context.RoomFacilitytbs
                                               .Where(rf => rf.RoomId == roomid)
                                               .ToListAsync();

            
            if (roomFacilities == null || !roomFacilities.Any())
            {
                return NotFound($"No facilities found for room ID: {roomid}");
            }

            
            _context.RoomFacilitytbs.RemoveRange(roomFacilities);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool RoomFacilitytbExists(int id)
        {
            return (_context.RoomFacilitytbs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
