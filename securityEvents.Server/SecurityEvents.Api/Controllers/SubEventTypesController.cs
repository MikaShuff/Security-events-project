//SubEventTypesController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecurityEvents.Api.Data;
using SecurityEvents.Api.Dtos;

namespace SecurityEvents.Api.Controllers;

[ApiController]
[Route("api/sub-event-types")]
public class SubEventTypesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LookupItemDto>>> Get([FromQuery] int? eventType)
    {
        var query = db.SubEventsTypes.AsNoTracking();

        if (eventType.HasValue)
        {
            query = query.Where(x => x.EventType == eventType.Value);
        }

        var data = await query
            .OrderBy(x => x.SubEventId)
            .Select(x => new LookupItemDto(
                x.SubEventId,
                x.SubEventName
            ))
            .ToListAsync();

        return Ok(data);
    }
}
