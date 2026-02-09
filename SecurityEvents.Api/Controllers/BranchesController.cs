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
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] BranchUpdateDto dto)
    {
        var entity = await db.TAs400Branches.FindAsync(id);
        if (entity == null)
            return NotFound();

        // עדכון חלקי – רק מה שנשלח
        if (dto.AbSnifName != null)
            entity.AbSnifName = dto.AbSnifName;

        if (dto.AbReshetId.HasValue)
            entity.AbReshetId = dto.AbReshetId.Value;

        if (dto.AbEshkolId.HasValue)
            entity.AbEshkolId = dto.AbEshkolId.Value;

        if (dto.AbUpdated != null)
            entity.AbUpdated = dto.AbUpdated;

        if (dto.AbUpdateId.HasValue)
            entity.AbUpdateId = dto.AbUpdateId.Value;

        await db.SaveChangesAsync();
        return Ok(entity);
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var branch = await db.TAs400Branches.FindAsync(id);
        if (branch == null)
            return NotFound();

        db.TAs400Branches.Remove(branch);
        await db.SaveChangesAsync();

        return NoContent();
    }
}

