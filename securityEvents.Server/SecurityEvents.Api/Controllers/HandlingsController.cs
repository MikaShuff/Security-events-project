using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecurityEvents.Api.Data;
using SecurityEvents.Api.Dtos;
using SecurityEvents.Api.Models;

namespace SecurityEvents.Api.Controllers;

[ApiController]
[Route("api/handlings")]
public class HandlingsController(AppDbContext db) : ControllerBase
{
    // GET /api/handlings  → מחזיר LookupItemDto (id/name) כפי שקיים אצלך
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

    // GET /api/handlings/{id}  → מחזיר Entity מלא (לשימוש עתידי/דיבאג)
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Handling>> GetById(int id)
    {
        var entity = await db.Handlings.FindAsync(id);
        if (entity == null) return NotFound();
        return Ok(entity);
    }

    // POST /api/handlings  → יצירת טיפול חדש (HandlingType לא נוצר אוטומטית!)
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] HandlingCreateDto dto)
    {
        if (dto.HandlingType <= 0)
            return BadRequest("HandlingType חייב להיות גדול מ-0");

        if (string.IsNullOrWhiteSpace(dto.HandlingName))
            return BadRequest("HandlingName נדרש");

        var exists = await db.Handlings.FindAsync(dto.HandlingType);
        if (exists != null)
            return Conflict($"HandlingType {dto.HandlingType} כבר קיים");

        var entity = new Handling
        {
            HandlingType = dto.HandlingType,
            HandlingName = dto.HandlingName
        };

        db.Handlings.Add(entity);
        await db.SaveChangesAsync();

        // אם חשוב לך Location header, אפשר להחזיר CreatedAtAction(GetById)
        return CreatedAtAction(nameof(GetById), new { id = entity.HandlingType }, entity);
    }

    // PUT /api/handlings/{id}  → עדכון שם
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] HandlingUpdateDto dto)
    {
        var entity = await db.Handlings.FindAsync(id);
        if (entity == null) return NotFound();

        if (string.IsNullOrWhiteSpace(dto.HandlingName))
            return BadRequest("HandlingName נדרש לעדכון");

        entity.HandlingName = dto.HandlingName;
        await db.SaveChangesAsync();
        return Ok(entity);
    }

    // DELETE /api/handlings/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await db.Handlings.FindAsync(id);
        if (entity == null) return NotFound();

        db.Handlings.Remove(entity);
        await db.SaveChangesAsync();
        return NoContent();
    }
}