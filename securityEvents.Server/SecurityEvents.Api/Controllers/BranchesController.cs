//BranchesController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecurityEvents.Api.Data;
using SecurityEvents.Api.Dtos;
using SecurityEvents.Api.Models;

namespace SecurityEvents.Api.Controllers;

[ApiController]
[Route("api/branches")]
public class BranchesController(AppDbContext db) : ControllerBase
{
    //GET
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

    // GET /api/branches/123/details
    [HttpGet("{branchId:int}/details")]
    public async Task<ActionResult<BranchDetailsDto>> GetDetails(int branchId)
    {
        var result = await db.TAs400Branches
            .Where(b => b.AbSnifId == branchId)
            .Select(b => new BranchDetailsDto
            {
                BranchId = b.AbSnifId,
                BranchName = b.AbSnifName,

                CompanyId = b.AbReshetId,                   // קוד החברה
                CompanyName = db.TAs400Companies
                    .Where(c => c.AcHevraId == b.AbReshetId)
                    .Select(c => c.AcHevraName)
                    .FirstOrDefault(),                     // שם החברה

                CompanyShort = db.TAs400Companies
                    .Where(c => c.AcHevraId == b.AbReshetId)
                    .Select(c => c.AcShortName)
                    .FirstOrDefault(),

                EshkolId = b.AbEshkolId,
                EshkolName = db.TAs400Eshkols
                    .Where(e => e.AeEshkolId == b.AbEshkolId)
                    .Select(e => e.AeEshkolName)
                    .FirstOrDefault(),

                SecurityZoneId = b.Zones  // מתוך many-to-many
                    .Select(z => (int?)z.ZoneId)
                    .FirstOrDefault(),

                SecurityZoneName = b.Zones
                    .Select(z => z.ZoneName)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync();

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    //POST
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BranchCreateDto dto)
    {
        if (dto.AbSnifId <= 0)
            return BadRequest("AbSnifId חייב להיות גדול מ-0");

        if (string.IsNullOrWhiteSpace(dto.AbSnifName))
            return BadRequest("AbSnifName נדרש");

        var exists = await db.TAs400Branches.FindAsync(dto.AbSnifId);
        if (exists != null)
            return Conflict($"סניף עם קוד {dto.AbSnifId} כבר קיים");

        var branch = new TAs400Branch
        {
            AbSnifId = dto.AbSnifId,
            AbSnifName = dto.AbSnifName,
            AbReshetId = dto.AbReshetId,
            AbEshkolId = dto.AbEshkolId,
            AbUpdated = dto.AbUpdated,
            AbUpdateId = dto.AbUpdateId
        };

        db.TAs400Branches.Add(branch);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = branch.AbSnifId }, branch);
    }

    //PUT
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] BranchUpdateDto dto)
    {
        var entity = await db.TAs400Branches.FindAsync(id);
        if (entity == null)
            return NotFound();

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

    //DELETE
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