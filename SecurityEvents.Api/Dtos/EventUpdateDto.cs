namespace SecurityEvents.Api.Dtos;

public class EventUpdateDto
{
    public string? EventDesc { get; set; }
    public int? EventType { get; set; }
    public int? SubEventId { get; set; }
    public int? BranchNum { get; set; }
    public decimal? EventSum { get; set; }
    public int? HandleType { get; set; }
    public string? HandleDesc { get; set; }
    public int? OfficerId { get; set; }
    public string? Remark { get; set; }
    public int? StatusId { get; set; }
    public string? CeoRemark { get; set; }
    public string? CustomerTz { get; set; }
}