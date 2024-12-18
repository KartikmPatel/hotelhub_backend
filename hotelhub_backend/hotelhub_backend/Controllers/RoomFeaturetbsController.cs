﻿using System;
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

        [HttpGet("getRoomFeatureCountByHotel/{id}")]
        public async Task<ActionResult<int>> GetRoomFeatureCountByHotel(int id)
        {
            try
            {
                int featureCount = await (from feature in _context.Featurestbs
                                          join roomFeature in _context.RoomFeaturetbs on feature.Id equals roomFeature.FeatureId
                                          join room in _context.Roomtbs on roomFeature.RoomId equals room.Id
                                          where room.Hid == id
                                          select feature.Id).Distinct().CountAsync();

                return featureCount;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error fetching feature count: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error." });
            }
        }

        // GET: api/RoomFeaturetbs/getAllFeatureByHotel/5
        [HttpGet("getAllFeatureByHotel/{id}")]
        public async Task<ActionResult<IEnumerable<object>>> GetRoomFeatureByHotel(int id)
        {
            var features = await (from feature in _context.Featurestbs
                                  join roomFeature in _context.RoomFeaturetbs on feature.Id equals roomFeature.FeatureId
                                  join room in _context.Roomtbs on roomFeature.RoomId equals room.Id
                                  where room.Hid == id
                                  select new
                                  {
                                      feature.FeatureName,
                                      feature.Image
                                  }).Distinct().ToListAsync();

            if (features == null)
            {
                return NotFound();
            }

            return features;
        }

        // PUT: api/RoomFeaturetbs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("updateRoomFeatures/{roomId}")]
        public async Task<IActionResult> UpdateRoomFeatures(int roomId, [FromBody] List<int> featureIds)
        {
            // Find existing features linked to the room
            var existingRoomFeatures = _context.RoomFeaturetbs
                .Where(rf => rf.RoomId == roomId)
                .ToList();

            // Remove features that are no longer selected
            foreach (var existingFeature in existingRoomFeatures)
            {
                if (!featureIds.Contains(existingFeature.FeatureId))
                {
                    _context.RoomFeaturetbs.Remove(existingFeature);
                }
            }

            // Add new features that are selected but not in the existing database
            foreach (var featureId in featureIds)
            {
                if (!existingRoomFeatures.Any(rf => rf.FeatureId == featureId))
                {
                    _context.RoomFeaturetbs.Add(new RoomFeaturetb
                    {
                        RoomId = roomId,
                        FeatureId = featureId
                    });
                }
            }

            // Save changes to the database
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating features");
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
