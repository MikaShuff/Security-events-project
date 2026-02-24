// EventCreateDto.cs 
namespace SecurityEvents.Api.Dtos;

using System.ComponentModel.DataAnnotations;

public class EventCreateDto
{
    [Required]
    public DateTime EventDate { get; set; }

    [Range(1, int.MaxValue)]
    public int BranchNum { get; set; }

    [Range(1, int.MaxValue)]
    public int EventType { get; set; }

    [Range(1, int.MaxValue)]
    public int SubEventId { get; set; }

    [Range(1, int.MaxValue)]
    public int OfficerId { get; set; }

    [Range(1, int.MaxValue)]
    public int HandleType { get; set; }

    [Range(1, int.MaxValue)]
    public int StatusId { get; set; }

    public decimal? EventSum { get; set; }

    [Required, MinLength(3), MaxLength(1000)]
    public string EventDesc { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? HandleDesc { get; set; }

    [MaxLength(1000)]
    public string? Remark { get; set; }

    [MaxLength(1000)]
    public string? CeoRemark { get; set; }

    [Required, MaxLength(20)]
    public string CustomerTz { get; set; } = string.Empty;
}