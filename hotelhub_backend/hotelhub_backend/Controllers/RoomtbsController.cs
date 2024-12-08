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
        // GET: api/Roomtbs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetRoomtb(int id)
        {
            if (_context.Roomtbs == null)
            {
                return NotFound();
            }

            // Fetch room details excluding feedbacks
            var roomDetails = await _context.Roomtbs
                .Where(r => r.Id == id)
                .Select(r => new
                {
                    r.Id,
                    r.Roomcategoryid,
                    r.AdultCapacity,
                    r.ChildrenCapacity,
                    r.Quantity,
                    r.ActiveStatus,
                    r.Hid,
                    r.City,
                    r.Rent,
                    r.Discount,
                    r.FestivalId,
                    RoomCategory = r.Roomcategory != null ? new
                    {
                        r.Roomcategory.Id,
                        r.Roomcategory.CategoryName
                    } : null,
                    Facilities = r.RoomFacilitytbs.Select(f => new
                    {
                        f.Facility.Id,
                        f.Facility.FacilityName,
                        f.Facility.Image
                    }).ToList(), // Materialize facilities as a list
                    Features = r.RoomFeaturetbs.Select(f => new
                    {
                        f.Feature.Id,
                        f.Feature.FeatureName,
                        f.Feature.Image
                    }).ToList(), // Materialize features as a list
                    Images = r.RoomImagetbs.Select(img => new
                    {
                        img.Id,
                        img.Image
                    }).ToList() // Materialize images as a list
                })
                .FirstOrDefaultAsync();

            if (roomDetails == null)
            {
                return NotFound();
            }

            // Fetch top 5 feedbacks separately
            var feedbacks = await _context.Feedbacktbs
                .Where(f => f.RoomId == id)
                .OrderByDescending(f => f.Rating)
                .Take(5)
                .Select(f => new
                {
                    f.Id,
                    f.Comments,
                    f.Rating,
                    f.Hid,
                    f.RoomId,
                    UserName = f.User != null ? f.User.Name : "Unknown",
                    Image = f.User != null ? f.User.Image : "Unknown",
                    f.ReadStatus
                })
                .ToListAsync(); // Materialize feedbacks as a list

            // Combine room details with feedbacks
            var finalDetails = new
            {
                roomDetails.Id,
                roomDetails.Roomcategoryid,
                roomDetails.AdultCapacity,
                roomDetails.ChildrenCapacity,
                roomDetails.Quantity,
                roomDetails.ActiveStatus,
                roomDetails.Hid,
                roomDetails.City,
                roomDetails.Rent,
                roomDetails.Discount,
                roomDetails.FestivalId,
                roomDetails.RoomCategory,
                roomDetails.Facilities,
                roomDetails.Features,
                roomDetails.Images,
                Feedbacks = feedbacks
            };

            return Ok(finalDetails);
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

        [HttpGet("topratedhotels")]
        public async Task<IActionResult> GetTopRatedHotels()
        {
            try
            {
                var topHotels = await _context.Hoteltbs
                    .Where(h => h.IsApproved == 1) // Assuming 1 indicates approved hotels
                    .Select(h => new
                    {
                        h.Id,
                        h.Hname,
                        h.City,
                        h.Image,
                        AverageRating = Math.Round(_context.Feedbacktbs
                            .Where(r => r.Hid == h.Id)
                            .Average(r => (double?)r.Rating) ?? 0
                            )
                    })
                    .OrderByDescending(h => h.AverageRating)
                    .Take(4)
                    .ToListAsync();

                return Ok(topHotels);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("gethotelsbycity")]
        public async Task<IActionResult> GetHotelsByCity([FromQuery] string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                return BadRequest(new { message = "City is required." });
            }

            // Get the list of hotel IDs based on the city
            var hotelIds = await _context.Roomtbs
                                        .Where(r => r.City.ToLower() == city.ToLower())
                                        .Select(r => r.Hid)
                                        .Distinct()
                                        .ToListAsync();

            if (hotelIds == null || hotelIds.Count == 0)
            {
                return NotFound(new { message = "No hotels found for the specified city." });
            }

            // Get hotel details along with average feedback rating based on the hotel IDs
            var hotels = await _context.Hoteltbs
                           .Where(h => hotelIds.Contains(h.Id))
                           .Select(h => new
                           {
                               h.Id,
                               h.Hname,
                               h.Image,
                               // Calculate and round average rating from the Feedbacktb table
                               Rating = Math.Round(
                                   _context.Feedbacktbs
                                           .Where(f => f.Hid == h.Id && f.Rating.HasValue)
                                           .Average(f => f.Rating) ?? 0 // Handle null case with ?? 0
                               )
                           })
                           .ToListAsync();

            return Ok(hotels);
        }

        [HttpGet("searchroomsbyhotel/{hid}")]
        public IActionResult GetRoomsByFilter(int hid, [FromQuery] string city, [FromQuery] int adultCapacity, [FromQuery] int childCapacity)
        {
            if (string.IsNullOrEmpty(city))
            {
                return BadRequest("City is required.");
            }

            var filteredRooms = _context.Roomtbs
                .Where(room => room.City == city &&
                               room.Hid == hid &&
                               room.AdultCapacity >= adultCapacity &&
                               room.ChildrenCapacity >= childCapacity)
                .Select(room => new
                {
                    room.Id,
                    CategoryName = _context.RoomCategorytbs
                                           .Where(category => category.Id == room.Roomcategoryid)
                                           .Select(category => category.CategoryName)
                                           .FirstOrDefault(),
                    room.AdultCapacity,
                    room.ChildrenCapacity,
                    room.Quantity,
                    room.ActiveStatus,
                    room.Hid,
                    room.City,
                    room.Rent,
                    room.Discount,
                    room.FestivalId,
                    FirstImage = _context.RoomImagetbs
                                         .Where(image => image.Roomid == room.Id)
                                         .OrderBy(image => image.Id)
                                         .Select(image => image.Image)
                                         .FirstOrDefault(),
                    Facilities = _context.RoomFacilitytbs
                                         .Where(rf => rf.RoomId == room.Id)
                                         .Select(rf => rf.Facility.FacilityName)
                                         .ToList(),
                    Features = _context.RoomFeaturetbs
                                       .Where(rf => rf.RoomId == room.Id)
                                       .Select(rf => rf.Feature.FeatureName)
                                       .ToList(),
                    AverageRating = Math.Round(
                            _context.Feedbacktbs
                        .Where(feedback => feedback.RoomId == room.Id && feedback.Rating.HasValue)
                        .Average(feedback => feedback.Rating) ?? 0 // Handle null with default value of 0
            )
                })
                .ToList();

            if (!filteredRooms.Any())
            {
                return NotFound("No rooms match the given criteria.");
            }

            return Ok(filteredRooms);
        }

        [HttpGet("search-by-category")]
        public IActionResult SearchByCategory(int categoryId, string city, int? hid, int? adultCapacity, int? childQuantity)
        {
            var query = _context.Roomtbs.AsQueryable();

            if (!string.IsNullOrEmpty(city))
                query = query.Where(r => r.City == city);

            if (hid.HasValue)
                query = query.Where(r => r.Hid == hid);

            if (adultCapacity.HasValue)
                query = query.Where(r => r.AdultCapacity >= adultCapacity.Value);

            if (childQuantity.HasValue)
                query = query.Where(r => r.ChildrenCapacity >= childQuantity.Value);

            query = query.Where(r => r.Roomcategoryid == categoryId);

            return Ok(FormatRoomResponse(query));
        }

        [HttpGet("search-by-rating")]
        public IActionResult SearchByRating(decimal rating, string city, int? hid, int? adultCapacity, int? childQuantity)
        {
            var query = _context.Roomtbs.AsQueryable();

            if (!string.IsNullOrEmpty(city))
                query = query.Where(r => r.City == city);

            if (hid.HasValue)
                query = query.Where(r => r.Hid == hid);

            if (adultCapacity.HasValue)
                query = query.Where(r => r.AdultCapacity >= adultCapacity.Value);

            if (childQuantity.HasValue)
                query = query.Where(r => r.ChildrenCapacity >= childQuantity.Value);

            query = query.Where(r => _context.Feedbacktbs
                .Where(f => f.RoomId == r.Id && f.Rating.HasValue)
                .Average(f => (double?)f.Rating) >= (double)rating);

            return Ok(FormatRoomResponse(query));
        }

        [HttpGet("search-by-status")]
        public IActionResult SearchByStatus(bool isActive, string city, int? hid, int? adultCapacity, int? childQuantity)
        {
            var query = _context.Roomtbs.AsQueryable();

            if (!string.IsNullOrEmpty(city))
                query = query.Where(r => r.City == city);

            if (hid.HasValue)
                query = query.Where(r => r.Hid == hid);

            if (adultCapacity.HasValue)
                query = query.Where(r => r.AdultCapacity >= adultCapacity.Value);

            if (childQuantity.HasValue)
                query = query.Where(r => r.ChildrenCapacity >= childQuantity.Value);

            query = query.Where(r => r.ActiveStatus == (isActive ? 1 : 0));

            return Ok(FormatRoomResponse(query));
        }

        [HttpGet("search-by-facilities-and-features")]
        public IActionResult SearchByFacilitiesAndFeatures(
    [FromQuery] List<int> facilityIds,
    [FromQuery] List<int> featureIds,
    string city,
    int? hid,
    int? adultCapacity,
    int? childQuantity)
        {
            var query = _context.Roomtbs.AsQueryable();

            if (!string.IsNullOrEmpty(city))
                query = query.Where(r => r.City == city);

            if (hid.HasValue)
                query = query.Where(r => r.Hid == hid);

            if (adultCapacity.HasValue)
                query = query.Where(r => r.AdultCapacity >= adultCapacity.Value);

            if (childQuantity.HasValue)
                query = query.Where(r => r.ChildrenCapacity >= childQuantity.Value);

            if (facilityIds != null && facilityIds.Any())
                query = query.Where(r => r.RoomFacilitytbs.Any(f => facilityIds.Contains(f.FacilityId)));

            if (featureIds != null && featureIds.Any())
                query = query.Where(r => r.RoomFeaturetbs.Any(f => featureIds.Contains(f.FeatureId)));

            return Ok(FormatRoomResponse(query));
        }



        private List<object> FormatRoomResponse(IQueryable<Roomtb> query)
        {
            return query.Select(room => new
            {
                room.Id,
                room.City,
                room.Hid,
                room.AdultCapacity,
                room.ChildrenCapacity,
                room.Quantity,
                room.ActiveStatus,
                room.Rent,
                room.Discount,
                room.Roomcategory.CategoryName,
                FirstImage = _context.RoomImagetbs
                    .Where(image => image.Roomid == room.Id)
                    .OrderBy(image => image.Id)
                    .Select(image => image.Image)
                    .FirstOrDefault(),
                Facilities = room.RoomFacilitytbs.Select(f => f.Facility.FacilityName).ToList(),
                Features = room.RoomFeaturetbs.Select(f => f.Feature.FeatureName).ToList(),
                AverageRating = Math.Round(_context.Feedbacktbs
                    .Where(f => f.RoomId == room.Id && f.Rating.HasValue)
                    .Average(f => f.Rating) ?? 0)
            })
            .Cast<object>()  // Explicitly cast to object
            .ToList();
        }

    }

}
