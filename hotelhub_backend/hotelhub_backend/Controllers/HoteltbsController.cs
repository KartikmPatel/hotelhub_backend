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
using static System.Net.Mime.MediaTypeNames;

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

        [HttpGet("gethotelcount")]
        public async Task<int> gethotelcount()
        {
            int hotelcount = await _context.Hoteltbs.CountAsync();
            return hotelcount;
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
        public async Task<ActionResult<Hoteltb>> PostHoteltb([FromForm] string hname, [FromForm] string email, [FromForm] string password, [FromForm] IFormFile image, [FromForm] string city)
        {
            try
            {
                // Check if an image is provided
                if (image != null && image.Length > 0)
                {
                    // Define the upload path (ensure it's correctly set up)
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

                    // Ensure the uploads folder exists
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    // Generate the file path where the image will be saved
                    var filePath = Path.Combine(uploadPath, image.FileName);

                    // Save the image file to the server
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    // Create a new Featurestb instance
                    var hoteltb = new Hoteltb
                    {
                        Hname = hname,
                        Email = email,
                        Password = password,
                        Image = image.FileName,      // Store only the filename in the database
                        City = city
                    };

                    if (_context.Hoteltbs == null)
                    {
                        return Problem("Entity set 'hotelhubContext.Hoteltbs'  is null.");
                    }
                    hoteltb.Password = HashPassword(hoteltb.Password);
                    _context.Hoteltbs.Add(hoteltb);
                    await _context.SaveChangesAsync();

                    return CreatedAtAction("GetHoteltb", new { id = hoteltb.Id }, hoteltb);

                }
                else
                {
                    return BadRequest("No file uploaded.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
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
        public async Task<IActionResult> Login([FromBody] Dictionary<string, string> loginData)
        {
            if (!loginData.ContainsKey("email") || !loginData.ContainsKey("password"))
            {
                return BadRequest(new { message = "Email and password are required." });
            }

            var email = loginData["email"];
            var password = loginData["password"];

            var hotel = await _context.Hoteltbs.FirstOrDefaultAsync(u => u.Email == email);

            if (hotel == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            string hashedPassword = HashPassword(password);

            if (hotel.Password != hashedPassword)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            if (hotel.IsApproved == 0)
            {
                return Unauthorized(new { message = "Your account is not approved by the admin." });
            }

            return Ok(new { message = "Login successful." });
        }

        [HttpPost("approve")]
        public async Task<IActionResult> ApproveHotel([FromBody] Dictionary<string, int> request)
        {
            if (!request.ContainsKey("id"))
            {
                return BadRequest(new { message = "Hotel ID is required." });
            }

            int hotelId = request["id"];

            var hotel = await _context.Hoteltbs.FindAsync(hotelId);
            if (hotel == null)
            {
                return NotFound(new { message = "Hotel not found." });
            }

            hotel.IsApproved = 1;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Hotel approved successfully." });
        }

        [HttpPost("gethid")]
        public async Task<IActionResult> GetHotelId([FromBody] Dictionary<string, string> emailData)
        {
            if (!emailData.ContainsKey("email"))
            {
                return BadRequest("Email is required.");
            }

            var email = emailData["email"];

            var hotel = await _context.Hoteltbs.FirstOrDefaultAsync(u => u.Email == email);

            if (hotel == null)
            {
                return NotFound("Hotel not found.");
            }

            return Ok(new { HotelId = hotel.Id });
        }

    }
}