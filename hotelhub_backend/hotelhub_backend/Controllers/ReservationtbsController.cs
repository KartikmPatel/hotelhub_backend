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
                .Where(r => r.RoomId == rid && r.CheckOut > checkIn)
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
                    CheckOut = bookingRequest.CheckOut
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

        [HttpPost("updatequantity")]
        public async Task<IActionResult> UpdateQuantity([FromBody] Dictionary<string, int> qtyData)
        {
            if (qtyData == null || !qtyData.ContainsKey("roomid") || !qtyData.ContainsKey("qty"))
            {
                return BadRequest(new { message = "Invalid data. Please provide 'roomid' and 'qty'." });
            }

            try
            {
                var roomid = qtyData["roomid"];
                var qty = qtyData["qty"];

                // Find the room by ID
                var room = await _context.Roomtbs.FirstOrDefaultAsync(r => r.Id == roomid);
                if (room == null)
                {
                    return NotFound(new { message = "Room not found." });
                }

                // Update the room quantity
                room.Quantity -= qty;

                // Save changes to the database
                _context.Roomtbs.Update(room);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Quantity updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

    }
}
