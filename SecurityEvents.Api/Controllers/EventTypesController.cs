//EventTypesController.cs


using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecurityEvents.Api.Data;
using SecurityEvents.Api.Dtos;

namespace SecurityEvents.Api.Controllers;

[ApiController]
[Route("api/event-types")]
public class EventTypesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LookupItemDto>>> Get()
    {
        var data = await db.EventsTypes
            .AsNoTracking()
            .OrderBy(x => x.EventType)
            .Select(x => new LookupItemDto(
                x.EventType,
                x.EventName 
            ))
            .ToListAsync();

        return Ok(data);
    }
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] EventTypeUpdateDto dto)
    {
        var entity = await db.EventsTypes.FindAsync(id);
        if (entity == null)
            return NotFound();

       
        if (dto.EventName != null)
            entity.EventName = dto.EventName;

        await db.SaveChangesAsync();
        return Ok(entity);
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await db.EventsTypes.FindAsync(id);
        if (entity == null)
            return NotFound();

        db.EventsTypes.Remove(entity);
        await db.SaveChangesAsync();

        return NoContent();
    }
}
