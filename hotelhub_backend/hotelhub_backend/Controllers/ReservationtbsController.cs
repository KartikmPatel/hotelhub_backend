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
    public class ReservationtbsController : ControllerBase
    {
        private readonly hotelhubContext _context;

        public ReservationtbsController(hotelhubContext context)
        {
            _context = context;
        }

        // GET: api/Reservationtbs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservationtb>>> GetReservationtbs()
        {
          if (_context.Reservationtbs == null)
          {
              return NotFound();
          }
            return await _context.Reservationtbs.ToListAsync();
        }

        // GET: api/Reservationtbs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservationtb>> GetReservationtb(int id)
        {
          if (_context.Reservationtbs == null)
          {
              return NotFound();
          }
            var reservationtb = await _context.Reservationtbs.FindAsync(id);

            if (reservationtb == null)
            {
                return NotFound();
            }

            return reservationtb;
        }

        // PUT: api/Reservationtbs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReservationtb(int id, Reservationtb reservationtb)
        {
            if (id != reservationtb.Id)
            {
                return BadRequest();
            }

            _context.Entry(reservationtb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationtbExists(id))
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

        // POST: api/Reservationtbs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Reservationtb>> PostReservationtb(Reservationtb reservationtb)
        {
          if (_context.Reservationtbs == null)
          {
              return Problem("Entity set 'hotelhubContext.Reservationtbs'  is null.");
          }
            _context.Reservationtbs.Add(reservationtb);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReservationtb", new { id = reservationtb.Id }, reservationtb);
        }

        // DELETE: api/Reservationtbs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservationtb(int id)
        {
            if (_context.Reservationtbs == null)
            {
                return NotFound();
            }
            var reservationtb = await _context.Reservationtbs.FindAsync(id);
            if (reservationtb == null)
            {
                return NotFound();
            }

            _context.Reservationtbs.Remove(reservationtb);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReservationtbExists(int id)
        {
            return (_context.Reservationtbs?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpGet("getavailability/{rid}")]
        public IActionResult GetAvailibility(int rid, [FromQuery] DateTime checkIn)
        {
            if (_context.Reservationtbs == null)
            {
                return NotFound("Reservation table not found.");
            }


            // Query to get the count of reservations meeting the condition
            int count = _context.Reservationtbs
                .Where(r => r.RoomId == rid && r.CheckOut >= checkIn && r.BookingStatus==1)
                .Count();

            return Ok(new { AvailableReservationsCount = count });
        }

        [HttpPost("bookroom")]
        public async Task<IActionResult> BookRoom([FromBody] BookingRequest bookingRequest)
        {
            if (bookingRequest == null)
                return BadRequest(new { success = false, message = "Invalid booking data." });

            try
            {
                // Create a new reservation
                var reservation = new Reservationtb
                {
                    UserId = bookingRequest.UserId,
                    RoomId = bookingRequest.RoomId,
                    Hid = bookingRequest.HotelId,
                    Rent = bookingRequest.Rent,
                    CheckIn = bookingRequest.CheckIn,
                    CheckOut = bookingRequest.CheckOut,
                    BookingStatus = 1
                };

                // Add reservation to the database
                _context.Reservationtbs.Add(reservation);

                // Save changes to the database
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Room booked successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("getReservationByUser/{userId}")]
        public IActionResult GetReservationDetailsByUser(int userId)
        {
            var reservations = _context.Reservationtbs
                .Where(r => r.UserId == userId)
                .Select(r => new
                {
                    r.Id,
                    r.Hid,
                    r.CheckIn,
                    r.CheckOut,
                    r.RoomId,
                    r.Rent,
                    r.BookingStatus,
                    HotelName = r.HidNavigation.Hname, // Assuming Hoteltb has a property 'HotelName'
                    RoomCategory = r.Room.Roomcategory.CategoryName, // Assuming Roomtb has a navigation property to Roomcategory
                })
                .ToList();

            if (reservations.Count == 0)
            {
                return NotFound("No reservations found for this user.");
            }

            return Ok(reservations);
        }

        [HttpGet("cancelBooking/{uid}/{rid}")]
        public async Task<IActionResult> CancelBooking(int uid, int rid)
        {
            // Check if the reservation exists
            var reservation = await _context.Reservationtbs.FirstOrDefaultAsync(r => r.Id == rid);
            if (reservation == null)
            {
                return NotFound("Reservation not found.");
            }

            // Check if the user exists
            var user = await _context.Usertbs.FirstOrDefaultAsync(u => u.Id == uid);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Create the cancellation record
            var cancelBooking = new CancelBookingtb
            {
                Uid = uid, // User ID
                Revid = rid // Reservation ID
            };

            // Add the cancellation record to the CancelBookingtb table
            _context.CancelBookingtbs.Add(cancelBooking);

            //Optionally, update the reservation status if required (e.g., "Canceled" status)
            reservation.BookingStatus =0; // Update the status (this depends on your model)
            _context.Reservationtbs.Update(reservation);

            // Save changes to the database
            await _context.SaveChangesAsync();

            return Ok(reservation);
        }

        [HttpGet("getReservationByHotel/{hotelId}")]
        public IActionResult GetReservationDetailsByHotel(int hotelId)
        {
            var reservations = _context.Reservationtbs
                .Where(r => r.Hid == hotelId)  // Filter reservations by hotel ID
                .Select(r => new
                {
                    r.Id,
                    r.UserId,
                    r.CheckIn,
                    r.CheckOut,
                    r.RoomId,
                    r.Rent,
                    r.BookingStatus,
                    UserName = r.User.Name, // Assuming Usertb has a property 'Name'
                    RoomCategory = r.Room.Roomcategory.CategoryName,
                    city= r.Room.City// Assuming Roomtb has a navigation property to Roomcategory
                })
                .ToList();

            if (reservations.Count == 0)
            {
                return NotFound("No reservations found for this hotel.");
            }

            return Ok(reservations);
        }

        [HttpGet("getBookingCountByHotel/{hotelId}")]
        public async Task<int> getBookingCountByHotel(int hotelId)
        {
            // Count the number of reservations where the Hotel ID matches the provided hotelId
            int hotelBookingCount = await _context.Reservationtbs
                .Where(r => r.Hid == hotelId) // Filter by hotel ID
                .CountAsync();

            return hotelBookingCount;
        }

    }
}
