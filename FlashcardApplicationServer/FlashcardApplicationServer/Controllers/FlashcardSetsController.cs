using FlashcardApplicationServer.Data;
using FlashcardApplicationServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FlashcardApplicationServer.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class FlashcardSetsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FlashcardSetsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/FlashcardSets/all
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<FlashcardSet>>> GetAllFlashcardSets()
    {
        return await _context.FlashcardSets
        .Include(s => s.Flashcards)
        .ToListAsync();
    }

    // GET: api/FlashcardSets/mine
    [HttpGet("mine")]
    public async Task<ActionResult<IEnumerable<FlashcardSet>>> GetMyFlashcardSets()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return await _context.FlashcardSets
        .Where(s => s.UserId == userId)
        .Include(s => s.Flashcards)
        .ToListAsync();
    }

    // GET: api/FlashcardSets/5
    [HttpGet("{id}")]
    public async Task<ActionResult<FlashcardSet>> GetFlashcardSet(int id)
    {
        var flashcardSet = await _context.FlashcardSets
        .Include(s => s.Flashcards)
        .FirstOrDefaultAsync(s => s.Id == id);

        if (flashcardSet == null)
        {
            return NotFound();
        }

        return flashcardSet;
    }

    // PUT: api/FlashcardSets/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutFlashcardSet(int id, FlashcardSet flashcardSet)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var existingSet = await _context.FlashcardSets
            .Include(s => s.Flashcards)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (existingSet == null || existingSet.UserId != userId)
            return NotFound();

        existingSet.Title = flashcardSet.Title;

        _context.Flashcards.RemoveRange(existingSet.Flashcards);
        existingSet.Flashcards.Clear();

        if (flashcardSet.Flashcards != null)
        {
            foreach (var incoming in flashcardSet.Flashcards)
            {
                existingSet.Flashcards.Add(new Flashcard
                {
                    Term = incoming.Term,
                    Definition = incoming.Definition,
                    SetId = existingSet.Id
                });
            }
        }

        await _context.SaveChangesAsync();

        var updated = await _context.FlashcardSets
            .Include(s => s.Flashcards)
            .FirstOrDefaultAsync(s => s.Id == id);

        return Ok(updated);
    }

    // POST: api/FlashcardSets
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<FlashcardSet>> PostFlashcardSet(FlashcardSet flashcardSet)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        flashcardSet.UserId = userId;

        _context.FlashcardSets.Add(flashcardSet);
        await _context.SaveChangesAsync();

        if (flashcardSet.Flashcards != null)
        {
            // Assign set ID to each flashcard in new set
            foreach (var flashcard in flashcardSet.Flashcards)
            {
                flashcard.SetId = flashcardSet.Id;
            }

            await _context.SaveChangesAsync();
        }

        // Return with flashcards included
        var createdSet = await _context.FlashcardSets
            .Include(s => s.Flashcards)
            .FirstOrDefaultAsync(s => s.Id == flashcardSet.Id);

        return CreatedAtAction("GetFlashcardSet", new { id = flashcardSet.Id }, createdSet);
    }

    // DELETE: api/FlashcardSets/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFlashcardSet(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var flashcardSet = await _context.FlashcardSets
            .Include(s => s.Flashcards)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (flashcardSet == null || flashcardSet.UserId != userId)
            return NotFound();

        if (flashcardSet.Flashcards != null)
        {
            _context.Flashcards.RemoveRange(flashcardSet.Flashcards);
        }

        _context.FlashcardSets.Remove(flashcardSet);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
