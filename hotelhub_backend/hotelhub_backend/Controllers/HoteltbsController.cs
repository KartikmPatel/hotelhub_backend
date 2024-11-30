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
using Org.BouncyCastle.Crypto.Generators;
using System.Net.Mail;
using System.Net;

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
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHoteltb(int id, [FromForm] string hname, [FromForm] string email, [FromForm] string city, [FromForm] IFormFile? image)
        {
            // Fetch the existing hotel record by ID
            var hoteltb = await _context.Hoteltbs.FindAsync(id);
            if (hoteltb == null)
            {
                return NotFound("Hotel not found.");
            }

            // Update hotel details
            hoteltb.Hname = hname;
            hoteltb.Email = email;
            hoteltb.City = city;

            if (image != null && image.Length > 0)
            {
                try
                {
                    // Generate a unique filename for the uploaded file
                    var uniqueFileName = Guid.NewGuid() + Path.GetExtension(image.FileName);

                    // Define upload path (use environment variable or config)
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

                    // Ensure the uploads folder exists
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    // Define full file path
                    var filePath = Path.Combine(uploadPath, uniqueFileName);

                    // Delete the old image if it exists
                    if (!string.IsNullOrEmpty(hoteltb.Image))
                    {
                        var oldImagePath = Path.Combine(uploadPath, hoteltb.Image);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Save the new image
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    // Update the image filename in the database
                    hoteltb.Image = uniqueFileName;
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Image upload failed: {ex.Message}");
                }
            }

            // Mark entity as modified and save changes
            _context.Entry(hoteltb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HoteltbExists(id))
                {
                    return NotFound("Hotel record not found during update.");
                }
                throw;
            }

            return NoContent();
        }


        [HttpPut("changeHotelPassword/{id}")]
        public async Task<ActionResult<Hoteltb>> ChangePassword(int id, [FromForm] string password, [FromForm] string newpassword)
        {
            try
            {
                // Retrieve the hotel by ID
                var hoteltb = await _context.Hoteltbs.FindAsync(id);

                if (hoteltb == null)
                {
                    return NotFound($"Hotel with ID {id} not found.");
                }

                string op = HashPassword(password); // Hash the provided old password

                // Check if the old password matches the stored password (hashed)
                if (!op.Equals(hoteltb.Password))
                {
                    return BadRequest("The old password is incorrect.");
                }

                // Hash the new password and update
                hoteltb.Password = HashPassword(newpassword);

                // Save the changes to the database
                _context.Hoteltbs.Update(hoteltb);
                await _context.SaveChangesAsync();

                return Ok(hoteltb); // Return the updated hotel info
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
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

        [HttpPost("forgotpassword")]
        public async Task<IActionResult> ForgotPassword([FromForm] string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest(new { message = "Email is required." });
                }

                var user = await _context.Hoteltbs.FirstOrDefaultAsync(h => h.Email == email);
                if (user == null)
                {
                    return NotFound(new { message = "No user found with this email." });
                }

                // Construct the reset URL
                var resetUrl = $"{Request.Scheme}://{Request.Host}/reset-password";

                // Send the email with the reset link
                var subject = "Password Reset Request";
                var body = $@"
                    <p>Hi {user.Hname},</p>
                    <p>You requested to reset your password. Click the link below to reset your password:</p>
                    <a href='{resetUrl}'>Reset Password</a>
                    <p>If you did not request this, please ignore this email.</p>";

                // Try sending the email
                await SendEmailAsync(user.Email, subject, body);

                return Ok(new { message = "Password reset email sent successfully." });
            }
            catch (Exception ex)
            {
                // Log detailed error message
                Console.Error.WriteLine($"Error sending password reset email: {ex.Message}");
                Console.Error.WriteLine($"Stack Trace: {ex.StackTrace}");

                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(
                        Environment.GetEnvironmentVariable("pnaitik504@gmail.com"), // Get SMTP user from environment variables
                        Environment.GetEnvironmentVariable("qnpqwcohvtazvehb") // Get SMTP password from environment variables
                    ),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(Environment.GetEnvironmentVariable("pnaitik504@gmail.com")),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(toEmail);

                // Send the email asynchronously
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (SmtpException smtpEx)
            {
                Console.Error.WriteLine($"SMTP Error: {smtpEx.Message}");
                throw new Exception("Error while sending email. Please try again later.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"General Error: {ex.Message}");
                throw new Exception("An unexpected error occurred. Please try again later.");
            }
        }



        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromForm] string email, [FromForm] string newPassword)
        {
            try
            {
                // Find user by reset token (no expiration check)
                var user = await _context.Hoteltbs.FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    return BadRequest(new { message = "Invalid User." });
                }

                // Reset the password
                user.Password = HashPassword(newPassword);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Password reset successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
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