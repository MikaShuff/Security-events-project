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
}
