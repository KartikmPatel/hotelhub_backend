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
using System.Net.Mail;
using System.Net;

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
        public async Task<IActionResult> PutUsertb(int id, [FromForm] string name, [FromForm] string email, [FromForm] string mno, [FromForm] string city, [FromForm] string gender, [FromForm] IFormFile? image)
        {
            // Fetch the existing user
            var usertb = await _context.Usertbs.FindAsync(id);
            if (usertb == null)
            {
                return NotFound("User not found.");
            }

            // Update user details
            usertb.Name = name;
            usertb.Email = email;
            usertb.Mno = mno;
            usertb.City = city;
            usertb.Gender = gender;

            if (image != null && image.Length > 0)
            {
                try
                {
                    // Generate a unique filename
                    var uniqueFileName = Guid.NewGuid() + Path.GetExtension(image.FileName);

                    // Define upload path
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    // Delete the old image if it exists
                    if (!string.IsNullOrEmpty(usertb.Image))
                    {
                        var oldImagePath = Path.Combine(uploadPath, usertb.Image);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Save the new image
                    var filePath = Path.Combine(uploadPath, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    // Update the image filename
                    usertb.Image = uniqueFileName;
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Image upload failed: {ex.Message}");
                }
            }

            _context.Entry(usertb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Usertbs.Any(u => u.Id == id))
                {
                    return NotFound("User not found during update.");
                }
                throw;
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
                        return Problem("Entity set 'hotelhubContext.Usertbs'  is null.");
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

        [HttpPost("forgotpassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] Dictionary<string, string> emailData)
        {
            try
            {
                // Validate input
                if (!emailData.ContainsKey("email"))
                {
                    return BadRequest(new { message = "Email is required." });
                }

                var email = emailData["email"];

                // Check if user exists
                var user = await _context.Usertbs.FirstOrDefaultAsync(h => h.Email == email);
                if (user == null)
                {
                    return NotFound(new { message = "No user found with this email." });
                }

                // Construct the reset URL
                var resetUrl = $"http://localhost:4200/resetuserpassword?email={email}";

                // Email content
                var subject = "Password Reset Request";
                var body = $@"
            <p>Hi {user.Name},</p>
            <p>You requested to reset your password. Click the link below to reset your password:</p>
            <a href='{resetUrl}' target='_blank'>Reset Password</a>
            <p>If you did not request this, please ignore this email.</p>";

                // Send the email
                await SendEmailAsync(user.Email, subject, body);

                return Ok(new { message = "Password reset email sent successfully." });
            }
            catch (Exception ex)
            {
                // Log the error
                Console.Error.WriteLine($"Error sending password reset email: {ex.Message}");
                Console.Error.WriteLine($"Stack Trace: {ex.StackTrace}");

                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                // Hardcoded credentials for debugging (replace with environment variables in production)
                var smtpUser = "kartikmpatel0804@gmail.com";
                var smtpPassword = "amyvnsuxoraosiif"; // Use your Gmail App Password here

                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(smtpUser, smtpPassword),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpUser),
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
        public async Task<IActionResult> ResetPassword([FromBody] Dictionary<string, string> resetData)
        {
            try
            {
                if (!resetData.ContainsKey("email") || !resetData.ContainsKey("password"))
                {
                    return BadRequest(new { message = "Email and password are required." });
                }

                var email = resetData["email"];
                var newPassword = resetData["password"];

                // Find user by email
                var user = await _context.Usertbs.FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    return BadRequest(new { message = "Invalid user." });
                }

                // Reset the password (assumes HashPassword is a method for hashing passwords)
                user.Password = HashPassword(newPassword);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Password reset successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
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
