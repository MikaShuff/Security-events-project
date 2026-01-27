//OfficersController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecurityEvents.Api.Data;
using SecurityEvents.Api.Dtos;

namespace SecurityEvents.Api.Controllers;

[ApiController]
[Route("api/officers")]
public class OfficersController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LookupItemDto>>> Get()
    {
        var data = await db.Officers
            .AsNoTracking()
            .OrderBy(x => x.OfficerName)
            .Select(x => new LookupItemDto(
                x.OfficerId,
                x.OfficerName ?? x.OfficerId.ToString()
            ))
            .ToListAsync();

        return Ok(data);
    }
}
