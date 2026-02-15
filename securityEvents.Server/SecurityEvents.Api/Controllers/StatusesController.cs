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
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await db.Statuses.FindAsync(id);
        if (entity == null)
            return NotFound();

        db.Statuses.Remove(entity);
        await db.SaveChangesAsync();

        return NoContent();
    }
}
