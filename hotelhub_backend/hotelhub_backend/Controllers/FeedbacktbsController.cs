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
    public class FeedbacktbsController : ControllerBase
    {
        private readonly hotelhubContext _context;

        public FeedbacktbsController(hotelhubContext context)
        {
            _context = context;
        }

        // GET: api/Feedbacktbs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Feedbacktb>>> GetFeedbacktbs()
        {
          if (_context.Feedbacktbs == null)
          {
              return NotFound();
          }
            return await _context.Feedbacktbs.ToListAsync();
        }

        [HttpGet("getByHotel/{hid}")]
        public async Task<ActionResult<IEnumerable<Feedbacktb>>> getByHotel(int hid)
        {
            if (_context.Feedbacktbs == null)
            {
                return NotFound();
            }

            var feedbacks = await (from feedback in _context.Feedbacktbs
                                   where feedback.Hid == hid
                                   select new
                                   {
                                       feedback.Id,
                                       feedback.Comments,
                                       feedback.Rating,
                                       categoryname = feedback.Room.Roomcategory.CategoryName,
                                       username = feedback.User.Name,
                                       feedback.ReadStatus
                                   }).ToListAsync();
            if(feedbacks.Count==0) {
                return NotFound("No Feedback found for the given hotel ID.");
            }

            return Ok(feedbacks);
        }


        // GET: api/Feedbacktbs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Feedbacktb>> GetFeedbacktb(int id)
        {
          if (_context.Feedbacktbs == null)
          {
              return NotFound();
          }
            var feedbacktb = await _context.Feedbacktbs.FindAsync(id);

            if (feedbacktb == null)
            {
                return NotFound();
            }

            return feedbacktb;
        }

        // PUT: api/Feedbacktbs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFeedbacktb(int id, Feedbacktb feedbacktb)
        {
            if (id != feedbacktb.Id)
            {
                return BadRequest();
            }

            _context.Entry(feedbacktb).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FeedbacktbExists(id))
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

        // POST: api/Feedbacktbs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Feedbacktb>> PostFeedbacktb(Feedbacktb feedbacktb)
        {
          if (_context.Feedbacktbs == null)
          {
              return Problem("Entity set 'hotelhubContext.Feedbacktbs'  is null.");
          }
            _context.Feedbacktbs.Add(feedbacktb);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFeedbacktb", new { id = feedbacktb.Id }, feedbacktb);
        }

        // DELETE: api/Feedbacktbs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeedbacktb(int id)
        {
            if (_context.Feedbacktbs == null)
            {
                return NotFound();
            }
            var feedbacktb = await _context.Feedbacktbs.FindAsync(id);
            if (feedbacktb == null)
            {
                return NotFound();
            }

            _context.Feedbacktbs.Remove(feedbacktb);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FeedbacktbExists(int id)
        {
            return (_context.Feedbacktbs?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpGet("markAsRead/{id}")]
        public async Task<IActionResult> markAsRead(int id)
        {
            if (_context.Feedbacktbs == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacktbs.FindAsync(id);

            if(feedback.ReadStatus == 0)
            {
                feedback.ReadStatus = 1;
            }
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
