//HandlingsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecurityEvents.Api.Data;
using SecurityEvents.Api.Dtos;

namespace SecurityEvents.Api.Controllers;

[ApiController]
[Route("api/handlings")]
public class HandlingsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LookupItemDto>>> Get()
    {
        var data = await db.Handlings
            .AsNoTracking()
            .OrderBy(x => x.HandlingType)
            .Select(x => new LookupItemDto(
                x.HandlingType,
                x.HandlingName ?? x.HandlingType.ToString()
            ))
            .ToListAsync();

        return Ok(data);
    }
}
