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
    public class FeedbacktbsController : ControllerBase
    {
        private readonly hotelhubContext _context;

        public FeedbacktbsController(hotelhubContext context)
        {
            _context = context;
        }

        // GET: api/Feedbacktbs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Feedbacktb>>> GetFeedbacktbs()
        {
          if (_context.Feedbacktbs == null)
          {
              return NotFound();
          }
            return await _context.Feedbacktbs.ToListAsync();
        }

        [HttpGet("getFeedbackCountByHotel/{hid}")]
        public async Task<ActionResult<int>> GetFeedbackCountByHotel(int hid)
        {
            var feedbackscount = await (from feedback in _context.Feedbacktbs
                                   where feedback.Hid == hid
                                   select feedback.Id).CountAsync();
            return feedbackscount;
        }

        [HttpGet("getByHotel/{hid}")]
        public async Task<ActionResult<IEnumerable<Feedbacktb>>> getByHotel(int hid)
        {
            if (_context.Feedbacktbs == null)
            {
                return NotFound();
            }

            var feedbacks = await (from feedback in _context.Feedbacktbs
                                   where feedback.Hid == hid
                                   select new
                                   {
                                       feedback.Id,
                                       feedback.Comments,
                                       feedback.Rating,
                                       categoryname = feedback.Room.Roomcategory.CategoryName,
                                       username = feedback.User.Name,
                                       feedback.ReadStatus
                                   }).ToListAsync();
            if(feedbacks.Count==0) {
                return NotFound("No Feedback found for the given hotel ID.");
            }

            return Ok(feedbacks);
        }


        // GET: api/Feedbacktbs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Feedbacktb>> GetFeedbacktb(int id)
        {
          if (_context.Feedbacktbs == null)
          {
              return NotFound();
          }
            var feedbacktb = await _context.Feedbacktbs.FindAsync(id);

            if (feedbacktb == null)
            {
                return NotFound();
            }

            return feedbacktb;
        }

        // PUT: api/Feedbacktbs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFeedbacktb(int id, Feedbacktb feedbacktb)
        {
            if (id != feedbacktb.Id)
            {
                return BadRequest();
            }

            _context.Entry(feedbacktb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FeedbacktbExists(id))
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

        // POST: api/Feedbacktbs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Feedbacktb>> PostFeedbacktb(Feedbacktb feedbacktb)
        {
          if (_context.Feedbacktbs == null)
          {
              return Problem("Entity set 'hotelhubContext.Feedbacktbs'  is null.");
          }
            _context.Feedbacktbs.Add(feedbacktb);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFeedbacktb", new { id = feedbacktb.Id }, feedbacktb);
        }

        // DELETE: api/Feedbacktbs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeedbacktb(int id)
        {
            if (_context.Feedbacktbs == null)
            {
                return NotFound();
            }
            var feedbacktb = await _context.Feedbacktbs.FindAsync(id);
            if (feedbacktb == null)
            {
                return NotFound();
            }

            _context.Feedbacktbs.Remove(feedbacktb);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FeedbacktbExists(int id)
        {
            return (_context.Feedbacktbs?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpGet("markAsRead/{id}")]
        public async Task<IActionResult> markAsRead(int id)
        {
            if (_context.Feedbacktbs == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacktbs.FindAsync(id);

            if(feedback.ReadStatus == 0)
            {
                feedback.ReadStatus = 1;
            }
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("getfeedbackbyuser/{userId}")]
        public IActionResult GetFeedbackByUser(int userId)
        {
            try
            {
                var feedbacks = _context.Feedbacktbs
                    .Where(f => f.UserId == userId)
                    .Select(f => new
                    {
                        FeedbackId = f.Id,
                        Comments = f.Comments,
                        Rating = f.Rating,
                        HotelName = f.HidNavigation.Hname, // Assuming HotelName is the property in Hoteltb
                        RoomCategory = f.Room.Roomcategory.CategoryName, // Assuming Roomcategory is linked in Roomtb
                        city = f.Room.City,
                        ReadStatus = f.ReadStatus
                    })
                    .ToList();

                if (!feedbacks.Any())
                {
                    return NotFound(new { Message = "No feedback found for the specified user." });
                }

                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching feedback data.", Error = ex.Message });
            }
        }

        [HttpGet("GetHotels")]
        public IActionResult GetHotels()
        {
            try
            {
                var hotels = _context.Hoteltbs
                    .Select(h => new { h.Id, h.Hname })
                    .ToList();

                return Ok(hotels);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching hotels.", Error = ex.Message });
            }
        }

        [HttpGet("GetRoomCitiesByHotel/{hotelId}")]
        public IActionResult GetRoomCitiesByHotel(int hotelId)
        {
            try
            {
                var cities = _context.Roomtbs
                    .Where(r => r.Hid == hotelId)
                    .Select(r => r.City)
                    .Distinct()
                    .ToList();

                return Ok(cities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching room cities.", Error = ex.Message });
            }
        }

        [HttpGet("GetRoomCategoriesByHotelAndCity/{hotelId}/{city}")]
        public IActionResult GetRoomCategoriesByHotelAndCity(int hotelId, string city)
        {
            try
            {
                var categories = _context.Roomtbs
                    .Where(r => r.Hid == hotelId && r.City == city)
                    .Select(r => new { r.Roomcategory.Id, r.Roomcategory.CategoryName })
                    .Distinct()
                    .ToList();

                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching room categories.", Error = ex.Message });
            }
        }

        [HttpGet("rooms/find")]
        public IActionResult GetRoomId([FromQuery] int hotelId, [FromQuery] string city, [FromQuery] int roomCategoryId)
        {
            var room = _context.Roomtbs.FirstOrDefault(r =>
                r.Hid == hotelId && r.City == city && r.Roomcategoryid == roomCategoryId);

            if (room == null)
                return NotFound(new { message = "Room not found" });

            return Ok(new { roomId = room.Id });
        }

    }
}
