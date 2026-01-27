
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecurityEvents.Api.Data;
using SecurityEvents.Api.Models;

namespace SecurityEvents.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController(AppDbContext db) : ControllerBase
{
    // GET /api/events
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Event>>> GetAll()
    {
        // דוגמה: נקרא את 100 האירועים האחרונים לפי EventId
        var data = await db.Events
            .OrderByDescending(e => e.EventId)
            .Take(100)
            .ToListAsync();

        return Ok(data);
    }

    // GET /api/events/123
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Event>> GetById(int id)
    {
        var item = await db.Events.FindAsync(id);
        if (item is null) return NotFound();
        return Ok(item);
    }

    // בהמשך נוכל להוסיף POST/PUT/DELETE בהתאם להרשאות ולעיצוב הנתונים
}
