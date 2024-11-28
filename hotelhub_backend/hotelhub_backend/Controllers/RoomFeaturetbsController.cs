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
    public class RoomFeaturetbsController : ControllerBase
    {
        private readonly hotelhubContext _context;

        public RoomFeaturetbsController(hotelhubContext context)
        {
            _context = context;
        }

        // GET: api/RoomFeaturetbs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomFeaturetb>>> GetRoomFeaturetbs(int roomId)
        {
            if (_context.RoomFeaturetbs == null)
            {
                return NotFound();
            }

            var roomFeatures = await (from feature in _context.RoomFeaturetbs
                                      where feature.RoomId == roomId
                                      select feature).ToListAsync();

            
            return Ok(roomFeatures);
        }

        // GET: api/RoomFeaturetbs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RoomFeaturetb>> GetRoomFeaturetb(int id)
        {
          if (_context.RoomFeaturetbs == null)
          {
              return NotFound();
          }
            var roomFeaturetb = await _context.RoomFeaturetbs.FindAsync(id);

            if (roomFeaturetb == null)
            {
                return NotFound();
            }

            return roomFeaturetb;
        }

        // PUT: api/RoomFeaturetbs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoomFeaturetb(int id, RoomFeaturetb roomFeaturetb)
        {
            if (id != roomFeaturetb.Id)
            {
                return BadRequest();
            }

            _context.Entry(roomFeaturetb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomFeaturetbExists(id))
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

        // POST: api/RoomFeaturetbs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RoomFeaturetb>> PostRoomFeaturetb([FromBody] RoomFeaturetb request)
        {
            if (_context.RoomFeaturetbs == null)
            {
                return Problem("Entity set 'hotelhubContext.RoomFeaturetbs' is null.");
            }

            var roomFeaturetb = new RoomFeaturetb
            {
                FeatureId = request.FeatureId,
                RoomId = request.RoomId
            };

            _context.RoomFeaturetbs.Add(roomFeaturetb);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRoomFeaturetbs", new { roomId = roomFeaturetb.RoomId }, roomFeaturetb);
        }



        // DELETE: api/RoomFeaturetbs/5
        [HttpDelete("{roomid}")]
        public async Task<IActionResult> DeleteRoomFeaturetb(int roomid)
        {
            if (_context.RoomFeaturetbs == null)
            {
                return NotFound();
            }

            
            var roomFeatures = await _context.RoomFeaturetbs
                                             .Where(rf => rf.RoomId == roomid)
                                             .ToListAsync();

            
            if (roomFeatures == null || !roomFeatures.Any())
            {
                return NotFound($"No features found for room ID: {roomid}");
            }

            
            _context.RoomFeaturetbs.RemoveRange(roomFeatures);
            await _context.SaveChangesAsync();

            return NoContent(); 
        }


        private bool RoomFeaturetbExists(int id)
        {
            return (_context.RoomFeaturetbs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
