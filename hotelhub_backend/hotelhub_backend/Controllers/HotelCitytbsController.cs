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
    public class HotelCitytbsController : ControllerBase
    {
        private readonly hotelhubContext _context;

        public HotelCitytbsController(hotelhubContext context)
        {
            _context = context;
        }

        // GET: api/HotelCitytbs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HotelCitytb>>> GetHotelCitytb()
        {
          if (_context.HotelCitytb == null)
          {
              return NotFound();
          }
            return await _context.HotelCitytb.ToListAsync();
        }

        [HttpGet("getByHotel/{hid}")]
        public async Task<ActionResult<IEnumerable<HotelCitytb>>> getByHotel(int hid)
        {
            if(_context.HotelCitytb == null)
            {
                return NotFound();
            }

            var hotelCitys = await(from hotel in _context.HotelCitytb
                                   where hotel.Hid == hid
                                   select hotel).ToListAsync();

            return Ok(hotelCitys);
        }


        // GET: api/HotelCitytbs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HotelCitytb>> GetHotelCitytb(int id)
        {
          if (_context.HotelCitytb == null)
          {
              return NotFound();
          }
            var hotelCitytb = await _context.HotelCitytb.FindAsync(id);

            if (hotelCitytb == null)
            {
                return NotFound();
            }

            return hotelCitytb;
        }

        // PUT: api/HotelCitytbs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHotelCitytb(int id, HotelCitytb hotelCitytb)
        {
            if (id != hotelCitytb.Id)
            {
                return BadRequest();
            }

            _context.Entry(hotelCitytb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HotelCitytbExists(id))
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

        // POST: api/HotelCitytbs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<HotelCitytb>> PostHotelCitytb(HotelCitytb hotelCitytb)
        {
          if (_context.HotelCitytb == null)
          {
              return Problem("Entity set 'hotelhubContext.HotelCitytb'  is null.");
          }
            _context.HotelCitytb.Add(hotelCitytb);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHotelCitytb", new { id = hotelCitytb.Id }, hotelCitytb);
        }

        // DELETE: api/HotelCitytbs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotelCitytb(int id)
        {
            if (_context.HotelCitytb == null)
            {
                return NotFound();
            }
            var hotelCitytb = await _context.HotelCitytb.FindAsync(id);
            if (hotelCitytb == null)
            {
                return NotFound();
            }

            _context.HotelCitytb.Remove(hotelCitytb);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HotelCitytbExists(int id)
        {
            return (_context.HotelCitytb?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
