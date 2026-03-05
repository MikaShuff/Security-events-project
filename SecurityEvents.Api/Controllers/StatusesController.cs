using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecurityEvents.Api.Data;
using SecurityEvents.Api.Dtos;
using SecurityEvents.Api.Models;

namespace SecurityEvents.Api.Controllers;

[ApiController]
[Route("api/statuses")]
public class StatusesController(AppDbContext db) : ControllerBase
{
    // GET /api/statuses  → LookupItemDto (id + name)
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

    // GET /api/statuses/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Status>> GetById(int id)
    {
        var entity = await db.Statuses.FindAsync(id);
        if (entity == null) return NotFound();
        return Ok(entity);
    }

    // POST /api/statuses
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] StatusCreateDto dto)
    {
        if (dto.StatusId <= 0)
            return BadRequest("StatusId חייב להיות גדול מ-0");

        if (string.IsNullOrWhiteSpace(dto.StatusDescription))
            return BadRequest("StatusDescription נדרש");

        var exists = await db.Statuses.FindAsync(dto.StatusId);
        if (exists != null)
            return Conflict($"סטטוס עם קוד {dto.StatusId} כבר קיים");

        var entity = new Status
        {
            StatusId = dto.StatusId,
            StatusDescription = dto.StatusDescription
        };

        db.Statuses.Add(entity);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = entity.StatusId }, entity);
    }

    // PUT /api/statuses/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] StatusUpdateDto dto)
    {
        var entity = await db.Statuses.FindAsync(id);
        if (entity == null) return NotFound();

        if (!string.IsNullOrWhiteSpace(dto.StatusDescription))
            entity.StatusDescription = dto.StatusDescription;

        await db.SaveChangesAsync();

        return Ok(entity);
    }

    // DELETE /api/statuses/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await db.Statuses.FindAsync(id);
        if (entity == null) return NotFound();

        db.Statuses.Remove(entity);
        await db.SaveChangesAsync();

        return NoContent();
    }
}