//BranchesController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecurityEvents.Api.Data;
using SecurityEvents.Api.Dtos;

namespace SecurityEvents.Api.Controllers;

[ApiController]
[Route("api/branches")]
public class BranchesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LookupItemDto>>> Get()
    {
        var data = await db.TAs400Branches
            .AsNoTracking()
            .OrderBy(x => x.AbSnifName)
            .Select(x => new LookupItemDto(
                x.AbSnifId,
                x.AbSnifName ?? x.AbSnifId.ToString()
            ))
            .ToListAsync();

        return Ok(data);
    }
}

