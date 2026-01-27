//FormatController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecurityEvents.Api.Data;
using SecurityEvents.Api.Dtos;

namespace SecurityEvents.Api.Controllers;

[ApiController]
[Route("api/formats")]
public class FormatsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LookupItemDto>>> Get()
    {
        var data = await db.TAs400Companies
            .AsNoTracking()
            .OrderBy(x => x.AcHevraName)
            .Select(x => new LookupItemDto(
                x.AcHevraId,             
                x.AcHevraName ?? x.AcHevraId.ToString()
            ))
            .ToListAsync();

        return Ok(data);
    }
}
