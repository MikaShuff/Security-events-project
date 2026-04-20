//EventsController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecurityEvents.Api.Data;
using SecurityEvents.Api.Models;
using SecurityEvents.Api.Dtos;

namespace SecurityEvents.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController(AppDbContext db) : ControllerBase
{
    // GET /api/events
    [HttpGet]
    public async Task<ActionResult<object>> GetAll(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int? formatId,
        [FromQuery] int? branchNum,
        [FromQuery] int? zoneId,
        [FromQuery] int? officerId,
        [FromQuery] int? eventType,
        [FromQuery] int? subEventId,
        [FromQuery] int? handleType,
        [FromQuery] int? statusId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = db.Events.AsQueryable();

        // החל מסננים
        if (fromDate.HasValue)
            query = query.Where(e => e.EventDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(e => e.EventDate <= toDate.Value);

        if (eventType.HasValue)
            query = query.Where(e => e.EventType == eventType.Value);

        if (subEventId.HasValue)
            query = query.Where(e => e.SubEventId == subEventId.Value);

        if (branchNum.HasValue)
            query = query.Where(e => e.BranchNum == branchNum.Value);

        if (officerId.HasValue)
            query = query.Where(e => e.OfficerId == officerId.Value);

        if (handleType.HasValue)
            query = query.Where(e => e.HandleType == handleType.Value);

        if (statusId.HasValue)
            query = query.Where(e => e.StatusId == statusId.Value);

        // ספירת סך הכל התוצאות
        var totalCount = await query.CountAsync();

        // עימוד
        var data = await query
    .Join(
        db.Statuses,
        e => e.StatusId,
        s => s.StatusId,
        (e, s) => new EventListDto
        {
            EventId = e.EventId,
            EventDate = e.EventDate,
            BranchNum = e.BranchNum,
            EventType = e.EventType,
            SubEventId = e.SubEventId,
            OfficerId = e.OfficerId,
            HandleType = e.HandleType,
            EventSum = e.EventSum,
            EventDesc = e.EventDesc,

            StatusId = e.StatusId,
            StatusName = s.StatusDescription
        }
    )
    .OrderByDescending(e => e.EventDate)
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();

        // החזרת נתונים עם מטא-דאטה
        return Ok(new
        {
            data,
            totalCount,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        });
    }

    // GET /api/events/123
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Event>> GetById(int id)
    {
        var item = await db.Events.FindAsync(id);
        if (item is null) return NotFound();
        return Ok(item);
    }

    // POST /api/events

    [HttpPost]
    public async Task<ActionResult<Event>> Create([FromBody] EventCreateDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var officerExists = await db.Set<OfficersLookup>()
            .AnyAsync(o => o.OfficerId == dto.OfficerId);

        if (!officerExists)
            return BadRequest($"OfficerId {dto.OfficerId} does not exist");

        var entity = new Event
        {
            EventDate = dto.EventDate,
            BranchNum = dto.BranchNum,
            EventType = dto.EventType,
            SubEventId = dto.SubEventId,
            OfficerId = dto.OfficerId,
            CustomerTz = dto.CustomerTz,
            EventDesc = dto.EventDesc,
            EventSum = dto.EventSum ?? 0m,
            HandleType = dto.HandleType,
            HandleDesc = dto.HandleDesc,
            Remark = dto.Remark,
            StatusId = dto.StatusId,
            CeoRemark = dto.CeoRemark,
            DateModified = DateTime.Now
        };

        db.Events.Add(entity);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = entity.EventId }, entity);
    }

    // PUT /api/events/123
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] EventUpdateDto dto)
    {
        var entity = await db.Events.FindAsync(id);
        if (entity == null)
            return NotFound();

        
        if (dto.EventDesc != null) entity.EventDesc = dto.EventDesc;
        if (dto.EventType.HasValue) entity.EventType = dto.EventType.Value;
        if (dto.SubEventId.HasValue) entity.SubEventId = dto.SubEventId.Value;
        if (dto.BranchNum.HasValue) entity.BranchNum = dto.BranchNum.Value;
        if (dto.EventSum.HasValue) entity.EventSum = dto.EventSum.Value;
        if (dto.HandleType.HasValue) entity.HandleType = dto.HandleType.Value;
        if (dto.HandleDesc != null) entity.HandleDesc = dto.HandleDesc;
        if (dto.OfficerId.HasValue) entity.OfficerId = dto.OfficerId.Value;
        if (dto.Remark != null) entity.Remark = dto.Remark;
        if (dto.StatusId.HasValue) entity.StatusId = dto.StatusId.Value;
        if (dto.CeoRemark != null) entity.CeoRemark = dto.CeoRemark;
        if (dto.CustomerTz != null) entity.CustomerTz = dto.CustomerTz;

        entity.DateModified = DateTime.Now;

        await db.SaveChangesAsync();
        return Ok(entity);
    }

    // DELETE /api/events/123

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await db.Events.FindAsync(id);
        if (entity == null)
            return NotFound();

        db.Events.Remove(entity);
        await db.SaveChangesAsync();

        return NoContent();
    }



}
