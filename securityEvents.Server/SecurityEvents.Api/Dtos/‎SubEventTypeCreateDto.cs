namespace SecurityEvents.Api.Dtos
{
    public class SubEventTypeCreateDto
    {
        public int EventType { get; set; }          // חובה
        public int SubEventId { get; set; }         // חובה
        public string SubEventName { get; set; } = "";
    }
}