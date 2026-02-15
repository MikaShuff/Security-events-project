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
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] FormatUpdateDto dto)
    {
        var entity = await db.TAs400Companies.FindAsync(id);
        if (entity == null)
            return NotFound();

       
        if (dto.AcHevraName != null)
            entity.AcHevraName = dto.AcHevraName;

        if (dto.AcShortName != null)
            entity.AcShortName = dto.AcShortName;

        if (dto.AcUpdated != null)
            entity.AcUpdated = dto.AcUpdated;

        await db.SaveChangesAsync();
        return Ok(entity);
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await db.TAs400Companies.FindAsync(id);
        if (entity == null)
            return NotFound();

        db.TAs400Companies.Remove(entity);
        await db.SaveChangesAsync();

        return NoContent();
    }
}
