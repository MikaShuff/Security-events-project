public class EventListDto
{
    public int EventId { get; set; }
    public DateTime EventDate { get; set; }

    public int BranchNum { get; set; }
    public int EventType { get; set; }
    public int SubEventId { get; set; }
    public int OfficerId { get; set; }
    public int HandleType { get; set; }

    public int StatusId { get; set; }
    public string StatusName { get; set; } = string.Empty;

    public decimal EventSum { get; set; }
    public string EventDesc { get; set; } = "";
}
