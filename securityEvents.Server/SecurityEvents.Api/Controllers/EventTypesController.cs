using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecurityEvents.Api.Data;
using SecurityEvents.Api.Dtos;
using SecurityEvents.Api.Models; // <-- חשוב!

namespace SecurityEvents.Api.Controllers;

[ApiController]
[Route("api/event-types")]
public class EventTypesController : ControllerBase
{
    private readonly AppDbContext db;
    public EventTypesController(AppDbContext db) => this.db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LookupItemDto>>> Get()
    {
        var data = await db.EventsTypes
            .AsNoTracking()
            .OrderBy(x => x.EventType)
            .Select(x => new LookupItemDto(x.EventType, x.EventName))
            .ToListAsync();
        return Ok(data);
    }

    // POST: /api/event-types
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EventTypeDto dto)
    {
        if (dto.EventType <= 0) return BadRequest("EventType (id) חייב להיות > 0");
        if (string.IsNullOrWhiteSpace(dto.EventName)) return BadRequest("EventName נדרש");

        var exists = await db.EventsTypes.FindAsync(dto.EventType);
        if (exists != null) return Conflict($"EventType {dto.EventType} כבר קיים");

        var entity = new EventsType
        {
            EventType = dto.EventType,
            EventName = dto.EventName
        };

        db.EventsTypes.Add(entity);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = entity.EventType }, entity);
    }

    // PUT: /api/event-types/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] EventTypeUpdateDto dto)
    {
        var entity = await db.EventsTypes.FindAsync(id);
        if (entity == null) return NotFound();
        if (string.IsNullOrWhiteSpace(dto.EventName)) return BadRequest("EventName נדרש לעדכון");

        entity.EventName = dto.EventName;
        await db.SaveChangesAsync();
        return Ok(entity);
    }

    // DELETE: /api/event-types/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await db.EventsTypes.FindAsync(id);
        if (entity == null) return NotFound();

        db.EventsTypes.Remove(entity);
        await db.SaveChangesAsync();
        return NoContent();
    }
}