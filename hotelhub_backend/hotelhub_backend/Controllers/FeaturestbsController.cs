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
    public class FeaturestbsController : ControllerBase
    {
        private readonly hotelhubContext _context;

        public FeaturestbsController(hotelhubContext context)
        {
            _context = context;
        }

        // GET: api/Featurestbs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Featurestb>>> GetFeaturestbs()
        {
          if (_context.Featurestbs == null)
          {
              return NotFound();
          }
            return await _context.Featurestbs.ToListAsync();
        }

        // GET: api/Featurestbs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Featurestb>> GetFeaturestb(int id)
        {
          if (_context.Featurestbs == null)
          {
              return NotFound();
          }
            var featurestb = await _context.Featurestbs.FindAsync(id);

            if (featurestb == null)
            {
                return NotFound();
            }

            return featurestb;
        }

        // PUT: api/Featurestbs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFeaturestb(int id, [FromForm] string featureName, [FromForm] IFormFile? image)
        {
            // Fetch the existing feature record by ID
            var featurestb = await _context.Featurestbs.FindAsync(id);

            if (featurestb == null)
            {
                return NotFound();
            }

            // Update the feature name
            featurestb.FeatureName = featureName;

            // If a new image is provided, upload it
            if (image != null && image.Length > 0)
            {
                try
                {
                    // Define the upload path (ensure it's correctly set up)
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

                    // Ensure the uploads folder exists
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    // Generate the file path
                    var filePath = Path.Combine(uploadPath, image.FileName);

                    // Delete the old image if it exists
                    if (!string.IsNullOrEmpty(featurestb.Image))
                    {
                        var oldImagePath = Path.Combine(uploadPath, featurestb.Image);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Save the new image to the specified path
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    // Update the image filename in the database
                    featurestb.Image = image.FileName;
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }

            // Mark the entity as modified and save changes
            _context.Entry(featurestb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FeaturestbExists(id))
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


        // POST: api/Featurestbs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostFeaturestb([FromForm] string featureName, [FromForm] IFormFile image)
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
                    var featurestb = new Featurestb
                    {
                        FeatureName = featureName,  // Set the feature name
                        Image = image.FileName      // Store only the filename in the database
                    };

                    // Save feature details to the database
                    _context.Featurestbs.Add(featurestb);
                    await _context.SaveChangesAsync();

                    return CreatedAtAction("GetFeaturestb", new { id = featurestb.Id }, featurestb);
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


        // DELETE: api/Featurestbs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeaturestb(int id)
        {
            if (_context.Featurestbs == null)
            {
                return NotFound();
            }
            var featurestb = await _context.Featurestbs.FindAsync(id);
            if (featurestb == null)
            {
                return NotFound();
            }

            _context.Featurestbs.Remove(featurestb);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FeaturestbExists(int id)
        {
            return (_context.Featurestbs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
