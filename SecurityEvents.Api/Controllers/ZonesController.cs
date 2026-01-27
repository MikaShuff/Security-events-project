//ZonesController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecurityEvents.Api.Data;
using SecurityEvents.Api.Dtos;

namespace SecurityEvents.Api.Controllers;

[ApiController]
[Route("api/zones")]
public class ZonesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LookupItemDto>>> Get()
    {
        var data = await db.Zones
            .AsNoTracking()
            .OrderBy(x => x.ZoneId)
            .Select(x => new LookupItemDto(
                x.ZoneId,
                x.ZoneName ?? x.ZoneId.ToString()
            ))
            .ToListAsync();

        return Ok(data);
    }
}
