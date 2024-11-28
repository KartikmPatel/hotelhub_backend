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
    public class FacilitytbsController : ControllerBase
    {
        private readonly hotelhubContext _context;

        public FacilitytbsController(hotelhubContext context)
        {
            _context = context;
        }

        // GET: api/Facilitytbs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Facilitytb>>> GetFacilitytbs()
        {
          if (_context.Facilitytbs == null)
          {
              return NotFound();
          }
            return await _context.Facilitytbs.ToListAsync();
        }

        [HttpGet("getfacilitycount")]
        public async Task<int> getfacilitycount()
        {
            int facilitycount = await _context.Facilitytbs.CountAsync();
            return facilitycount;
        }

        // GET: api/Facilitytbs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Facilitytb>> GetFacilitytb(int id)
        {
          if (_context.Facilitytbs == null)
          {
              return NotFound();
          }
            var facilitytb = await _context.Facilitytbs.FindAsync(id);

            if (facilitytb == null)
            {
                return NotFound();
            }

            return facilitytb;
        }

        // PUT: api/Facilitytbs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFacilitytb(int id, [FromForm] string facilityName, [FromForm] IFormFile? image)
        {
            // Fetch the existing facility record by ID
            var facilitytb = await _context.Facilitytbs.FindAsync(id);

            if (facilitytb == null)
            {
                return NotFound();
            }

            // Update the facility name
            facilitytb.FacilityName = facilityName;

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
                    if (!string.IsNullOrEmpty(facilitytb.Image))
                    {
                        var oldImagePath = Path.Combine(uploadPath, facilitytb.Image);
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
                    facilitytb.Image = image.FileName;
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: {ex.Message}");
                }
            }

            // Mark the entity as modified and save changes
            _context.Entry(facilitytb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FacilitytbExists(id))
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


        // POST: api/Facilitytbs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostFacilitytb([FromForm] string facilityName, [FromForm] IFormFile image)
        {
            try
            {
                if (image != null && image.Length > 0)
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

                    // Save the file to the specified path
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    // Create a new Facilitytb instance
                    var facilitytb = new Facilitytb
                    {
                        FacilityName = facilityName,
                        Image = image.FileName  // Save only the filename in the database
                    };

                    // Save facility details to the database
                    _context.Facilitytbs.Add(facilitytb);
                    await _context.SaveChangesAsync();

                    return CreatedAtAction("GetFacilitytb", new { id = facilitytb.Id }, facilitytb);
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


        // DELETE: api/Facilitytbs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFacilitytb(int id)
        {
            if (_context.Facilitytbs == null)
            {
                return NotFound();
            }
            var facilitytb = await _context.Facilitytbs.FindAsync(id);
            if (facilitytb == null)
            {  
                return NotFound();
            }

            _context.Facilitytbs.Remove(facilitytb);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FacilitytbExists(int id)
        {
            return (_context.Facilitytbs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
