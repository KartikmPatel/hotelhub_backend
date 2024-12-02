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
    public class UsertbsController : ControllerBase
    {
        private readonly hotelhubContext _context;

        public UsertbsController(hotelhubContext context)
        {
            _context = context;
        }

        // GET: api/Usertbs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usertb>>> GetUsertbs()
        {
            if (_context.Usertbs == null)
            {
                return NotFound();
            }
            return await _context.Usertbs.ToListAsync();
        }

        // GET: api/Usertbs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usertb>> GetUsertb(int id)
        {
            if (_context.Usertbs == null)
            {
                return NotFound();
            }
            var usertb = await _context.Usertbs.FindAsync(id);

            if (usertb == null)
            {
                return NotFound();
            }

            return usertb;
        }

        // PUT: api/Usertbs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsertb(int id, Usertb usertb)
        {
            if (id != usertb.Id)
            {
                return BadRequest();
            }

            // Fetch the existing user from the database
            var existingUser = await _context.Usertbs.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

            if (existingUser == null)
            {
                return NotFound();
            }

            // Check if the password has been changed
            if (existingUser.Password != usertb.Password)
            {
                // Hash the new password before saving
                usertb.Password = HashPassword(usertb.Password);
            }

            // Mark the user entity as modified
            _context.Entry(usertb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsertbExists(id))
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

        // POST: api/Usertbs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Usertb>> PostUsertb([FromForm] string name, [FromForm] string email, [FromForm] string mno, [FromForm] string password, [FromForm] IFormFile image, [FromForm] string city, [FromForm] string gender)
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
                    var usertb = new Usertb
                    {
                        Name = name,
                        Email = email,
                        Mno = mno,
                        Password = password,
                        Image = image.FileName,      // Store only the filename in the database
                        City = city,
                        Gender = gender
                    };

                    if (_context.Usertbs == null)
                    {
                        return Problem("Entity set 'hotelhubContext.Hoteltbs'  is null.");
                    }
                    usertb.Password = HashPassword(usertb.Password);
                    _context.Usertbs.Add(usertb);
                    await _context.SaveChangesAsync();

                    return CreatedAtAction("GetUsertbs", new { id = usertb.Id }, usertb);

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

        // DELETE: api/Usertbs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsertb(int id)
        {
            if (_context.Usertbs == null)
            {
                return NotFound();
            }
            var usertb = await _context.Usertbs.FindAsync(id);
            if (usertb == null)
            {
                return NotFound();
            }

            _context.Usertbs.Remove(usertb);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsertbExists(int id)
        {
            return (_context.Usertbs?.Any(e => e.Id == id)).GetValueOrDefault();
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

            var user = await _context.Usertbs.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            string hashedPassword = HashPassword(password);

            if (user.Password != hashedPassword)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            return Ok(new { message = "Login successful." });
        }

        [HttpPost("getuid")]
        public async Task<IActionResult> GetUserId([FromBody] Dictionary<string, string> emailData)
        {
            if (!emailData.ContainsKey("email"))
            {
                return BadRequest("Email is required.");
            }

            var email = emailData["email"];

            var user = await _context.Usertbs.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return NotFound("Hotel not found.");
            }

            return Ok(new { UserId = user.Id });
        }
    }
}
