using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hotelhub_backend.Models;
using System.Text;
using System.Security.Cryptography;

namespace hotelhub_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoteltbsController : ControllerBase
    {
        private readonly hotelhubContext _context;

        public HoteltbsController(hotelhubContext context)
        {
            _context = context;
        }

        // GET: api/Hoteltbs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Hoteltb>>> GetHoteltbs()
        {
          if (_context.Hoteltbs == null)
          {
              return NotFound();
          }
            return await _context.Hoteltbs.ToListAsync();
        }

        // GET: api/Hoteltbs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Hoteltb>> GetHoteltb(int id)
        {
          if (_context.Hoteltbs == null)
          {
              return NotFound();
          }
            var hoteltb = await _context.Hoteltbs.FindAsync(id);

            if (hoteltb == null)
            {
                return NotFound();
            }

            return hoteltb;
        }

        // PUT: api/Hoteltbs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHoteltb(int id, Hoteltb hoteltb)
        {
            if (id != hoteltb.Id)
            {
                return BadRequest();
            }
            var existingHotel = await _context.Hoteltbs.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            _context.Entry(hoteltb).State = EntityState.Modified;

            if (existingHotel == null)
            {
                return NotFound();
            }

            if (existingHotel.Password != hoteltb.Password)
            {
                // Hash the new password before saving
                hoteltb.Password = HashPassword(hoteltb.Password);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HoteltbExists(id))
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

        // POST: api/Hoteltbs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Hoteltb>> PostHoteltb(Hoteltb hoteltb)
        {
          if (_context.Hoteltbs == null)
          {
              return Problem("Entity set 'hotelhubContext.Hoteltbs'  is null.");
          }
            hoteltb.Password = HashPassword(hoteltb.Password);
            _context.Hoteltbs.Add(hoteltb);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHoteltb", new { id = hoteltb.Id }, hoteltb);
        }

        // DELETE: api/Hoteltbs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHoteltb(int id)
        {
            if (_context.Hoteltbs == null)
            {
                return NotFound();
            }
            var hoteltb = await _context.Hoteltbs.FindAsync(id);
            if (hoteltb == null)
            {
                return NotFound();
            }

            _context.Hoteltbs.Remove(hoteltb);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HoteltbExists(int id)
        {
            return (_context.Hoteltbs?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert the byte array to a string.
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        [HttpPost("login")]
        public async Task<string> Login(string email, string password)
        {

            var hotel = await _context.Hoteltbs.FirstOrDefaultAsync(u => u.Email == email);

            if (hotel == null)
            {
                return "Invalid email or password.";
            }

            string hashedPassword = HashPassword(password);

            if (hotel.Password != hashedPassword)
            {
                return "Invalid email or password."; ;
            }

            return "login success";
        }
    }
}
