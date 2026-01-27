//StatusesController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecurityEvents.Api.Data;
using SecurityEvents.Api.Dtos;

namespace SecurityEvents.Api.Controllers;

[ApiController]
[Route("api/statuses")]
public class StatusesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LookupItemDto>>> Get()
    {
        var data = await db.Statuses
            .AsNoTracking()
            .OrderBy(x => x.StatusId)
            .Select(x => new LookupItemDto(
                x.StatusId,
                x.StatusDescription
            ))
            .ToListAsync();

        return Ok(data);
    }
}
