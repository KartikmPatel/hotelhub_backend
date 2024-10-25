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
    public class FestivalDiscountsController : ControllerBase
    {
        private readonly hotelhubContext _context;

        public FestivalDiscountsController(hotelhubContext context)
        {
            _context = context;
        }

        // GET: api/FestivalDiscounts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FestivalDiscount>>> GetFestivalDiscounts()
        {
          if (_context.FestivalDiscounts == null)
          {
              return NotFound();
          }
            return await _context.FestivalDiscounts.ToListAsync();
        }

        // GET: api/FestivalDiscounts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FestivalDiscount>> GetFestivalDiscount(int id)
        {
          if (_context.FestivalDiscounts == null)
          {
              return NotFound();
          }
            var festivalDiscount = await _context.FestivalDiscounts.FindAsync(id);

            if (festivalDiscount == null)
            {
                return NotFound();
            }

            return festivalDiscount;
        }

        // PUT: api/FestivalDiscounts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFestivalDiscount(int id, FestivalDiscount festivalDiscount)
        {
            if (id != festivalDiscount.Id)
            {
                return BadRequest();
            }

            _context.Entry(festivalDiscount).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FestivalDiscountExists(id))
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

        // POST: api/FestivalDiscounts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FestivalDiscount>> PostFestivalDiscount(FestivalDiscount festivalDiscount)
        {
          if (_context.FestivalDiscounts == null)
          {
              return Problem("Entity set 'hotelhubContext.FestivalDiscounts'  is null.");
          }
            _context.FestivalDiscounts.Add(festivalDiscount);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFestivalDiscount", new { id = festivalDiscount.Id }, festivalDiscount);
        }

        // DELETE: api/FestivalDiscounts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFestivalDiscount(int id)
        {
            if (_context.FestivalDiscounts == null)
            {
                return NotFound();
            }
            var festivalDiscount = await _context.FestivalDiscounts.FindAsync(id);
            if (festivalDiscount == null)
            {
                return NotFound();
            }

            _context.FestivalDiscounts.Remove(festivalDiscount);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FestivalDiscountExists(int id)
        {
            return (_context.FestivalDiscounts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
