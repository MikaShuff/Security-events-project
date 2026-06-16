//officersController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecurityEvents.Api.Data;
using SecurityEvents.Api.Dtos;
using SecurityEvents.Api.Models;

namespace SecurityEvents.Api.Controllers;

[ApiController]
[Route("api/officers")]
public class OfficersController(AppDbContext db) : ControllerBase
{
    // GET /api/officers  → LookupItemDto (id, name)

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LookupItemDto>>> Get()
    {
        var data = await db.Officers
            .AsNoTracking()
            .OrderBy(o => o.OfficerName)
            .Select(o => new LookupItemDto(
                o.OfficerId,
                o.OfficerName ?? o.OfficerId.ToString()
            ))
            .ToListAsync();

        return Ok(data);
    }


    // GET /api/officers/{id} → מחזיר Entity מלא (לעריכה מתקדמת/דיבאג)
    [HttpGet("{id:int}")]
    public async Task<ActionResult<object>> GetById(int id)
    {
        var o = await db.Officers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.OfficerId == id);

        if (o == null) return NotFound();
        return Ok(o);
    }

    // GET /api/officers/admin  -> רשימה עשירה למסך טבלאות מערכת (כולל ZoneName)
    [HttpGet("admin")]
    public async Task<ActionResult<IEnumerable<OfficerListDto>>> GetAdmin()
    {
        var data = await db.Officers
            .AsNoTracking()
            .Include(o => o.Zone)                 // <-- זה ה-JOIN בפועל דרך ZoneId
            .OrderBy(o => o.OfficerName)
            .Select(o => new OfficerListDto(
                o.OfficerId,
                o.OfficerName ?? o.OfficerId.ToString(),
                o.Zone != null ? o.Zone.ZoneName : ""   // <-- ZoneName מהטבלה Zones
            ))
            .ToListAsync();

        return Ok(data);
    }


    // POST /api/officers → יצירת קב"ט חדש
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] OfficerCreateDto dto)
    {
        if (dto.OfficerId <= 0)
            return BadRequest("OfficerId חייב להיות גדול מ-0");

        if (string.IsNullOrWhiteSpace(dto.OfficerName))
            return BadRequest("OfficerName נדרש");

        if (dto.ZoneId <= 0)
            return BadRequest("ZoneId נדרש וחייב להיות גדול מ-0");

        // בדיקת כפילות
        var exists = await db.Officers.FindAsync(dto.OfficerId);
        if (exists != null)
            return Conflict($"קיים כבר קב\"ט עם מזהה {dto.OfficerId}");

        var entity = new Officer
        {
            OfficerId = dto.OfficerId,
            OfficerName = dto.OfficerName,
            ZoneId = dto.ZoneId
        };

        db.Officers.Add(entity);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = entity.OfficerId }, entity);
    }

    // PUT /api/officers/{id} → קיים אצלך: עדכון שם/אזור
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

    // DELETE /api/officers/{id} → קיים אצלך
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