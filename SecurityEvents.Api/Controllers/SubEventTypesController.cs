using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecurityEvents.Api.Data;
using SecurityEvents.Api.Dtos;
using SecurityEvents.Api.Models;

namespace SecurityEvents.Api.Controllers;

[ApiController]
[Route("api/sub-event-types")]
public class SubEventTypesController : ControllerBase
{
    private readonly AppDbContext _db;

    public SubEventTypesController(AppDbContext db)
    {
        _db = db;
    }

    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LookupItemDto>>> Get([FromQuery] int? eventType)
    {
        var query = _db.SubEventsTypes.AsNoTracking();

        if (eventType.HasValue)
            query = query.Where(x => x.EventType == eventType.Value);

        var data = await query
            .OrderBy(x => x.SubEventId)
            .Select(x => new LookupItemDto(
                x.SubEventId,
                x.SubEventName
            ))
            .ToListAsync();

        return Ok(data);
    }

    [HttpGet("{eventType:int}/{subEventId:int}")]
    public async Task<ActionResult<SubEventsType>> GetById(int eventType, int subEventId)
    {
        var entity = await _db.SubEventsTypes
            .FirstOrDefaultAsync(x => x.EventType == eventType && x.SubEventId == subEventId);

        if (entity == null)
            return NotFound();

        return Ok(entity);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SubEventTypeCreateDto dto)
    {
        if (dto.EventType <= 0)
            return BadRequest("EventType חייב להיות גדול מ-0");

        if (dto.SubEventId <= 0)
            return BadRequest("SubEventId חייב להיות גדול מ-0");

        if (string.IsNullOrWhiteSpace(dto.SubEventName))
            return BadRequest("SubEventName נדרש");

        // בדיקת כפילות במפתח המורכב
        var exists = await _db.SubEventsTypes
            .AnyAsync(x => x.EventType == dto.EventType && x.SubEventId == dto.SubEventId);

        if (exists)
            return Conflict("תת־אירוע עם ה־EventType וה־SubEventId האלו כבר קיים.");

        var entity = new SubEventsType
        {
            EventType = dto.EventType,
            SubEventId = dto.SubEventId,
            SubEventName = dto.SubEventName
        };

        _db.SubEventsTypes.Add(entity);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new
        {
            eventType = entity.EventType,
            subEventId = entity.SubEventId
        }, entity);
    }

    [HttpPut("{eventType:int}/{subEventId:int}")]
    public async Task<IActionResult> Update(int eventType, int subEventId, [FromBody] SubEventTypeUpdateDto dto)
    {
        var entity = await _db.SubEventsTypes
            .FirstOrDefaultAsync(x => x.EventType == eventType && x.SubEventId == subEventId);

        if (entity == null)
            return NotFound();

        if (!string.IsNullOrWhiteSpace(dto.SubEventName))
            entity.SubEventName = dto.SubEventName;

        await _db.SaveChangesAsync();
        return Ok(entity);
    }
    [HttpDelete("{eventType:int}/{subEventId:int}")]
    public async Task<IActionResult> Delete(int eventType, int subEventId)
    {
        var entity = await _db.SubEventsTypes
            .FirstOrDefaultAsync(x => x.EventType == eventType && x.SubEventId == subEventId);

        if (entity == null)
            return NotFound();

        _db.SubEventsTypes.Remove(entity);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}