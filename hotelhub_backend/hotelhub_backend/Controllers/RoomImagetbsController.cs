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
    public class RoomImagetbsController : ControllerBase
    {
        private readonly hotelhubContext _context;

        public RoomImagetbsController(hotelhubContext context)
        {
            _context = context;
        }

        // GET: api/RoomImagetbs
        [HttpGet("roomimages/{roomid}")]
        public async Task<ActionResult<IEnumerable<RoomImagetb>>> GetRoomImagetbs(int roomid)
        {
            if (_context.RoomImagetbs == null)
            {
                return NotFound();
            }

            var roomImages = await (from image in _context.RoomImagetbs
                                    where image.Roomid == roomid
                                    select image).ToListAsync();

            return Ok(roomImages);
        }

        // GET: api/RoomImagetbs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RoomImagetb>> GetRoomImagetb(int id)
        {
          if (_context.RoomImagetbs == null)
          {
              return NotFound();
          }
            var roomImagetb = await _context.RoomImagetbs.FindAsync(id);

            if (roomImagetb == null)
            {
                return NotFound();
            }

            return roomImagetb;
        }

        // PUT: api/RoomImagetbs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoomImagetb(int id, RoomImagetb roomImagetb)
        {
            if (id != roomImagetb.Id)
            {
                return BadRequest();
            }

            _context.Entry(roomImagetb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomImagetbExists(id))
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

        // POST: api/RoomImagetbs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("upload")]
        public async Task<IActionResult> UploadRoomImages([FromForm] int roomId, [FromForm] ICollection<IFormFile> images)
        {
            try
            {
                if (images == null || images.Count == 0)
                {
                    return BadRequest("No images uploaded.");
                }

                // Define the upload path (ensure it's correctly set up)
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "roomimages");

                // Ensure the uploads folder exists
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var fileNames = new List<string>();

                // Loop through each image and save it to the server
                foreach (var image in images)
                {
                    if (image.Length > 0)
                    {
                        // Generate the file path
                        var filePath = Path.Combine(uploadPath, image.FileName);

                        // Save the file to the specified path
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        // Create a new RoomImagetb instance
                        var roomImagetb = new RoomImagetb
                        {
                            Roomid = roomId,  // Set the roomId from the request
                            Image = image.FileName  // Save only the filename in the database
                        };

                        // Save room image details to the database
                        _context.RoomImagetbs.Add(roomImagetb);
                        fileNames.Add(image.FileName);
                    }
                }

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Return the response with the list of uploaded file names
                return Ok(new { message = "Images uploaded successfully", fileNames });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        // DELETE: api/RoomImagetbs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoomImagetb(int id)
        {
            if (_context.RoomImagetbs == null)
            {
                return NotFound();
            }
            var roomImagetb = await _context.RoomImagetbs.FindAsync(id);
            if (roomImagetb == null)
            {
                return NotFound();
            }

            _context.RoomImagetbs.Remove(roomImagetb);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoomImagetbExists(int id)
        {
            return (_context.RoomImagetbs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
