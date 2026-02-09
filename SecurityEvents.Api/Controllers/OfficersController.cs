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
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] OfficerUpdateDto dto)
    {
        var officer = await db.Officers.FindAsync(id);
        if (officer == null)
            return NotFound();

        
        if (dto.OfficerName != null)
            officer.OfficerName = dto.OfficerName;

        if (dto.ZoneId.HasValue)
            officer.ZoneId = dto.ZoneId.Value;

        await db.SaveChangesAsync();
        return Ok(officer);
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await db.Officers.FindAsync(id);
        if (entity == null)
            return NotFound();

        db.Officers.Remove(entity);
        await db.SaveChangesAsync();

        return NoContent();
    }
}
