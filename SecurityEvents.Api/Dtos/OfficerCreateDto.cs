namespace SecurityEvents.Api.Dtos
{
    public class OfficerCreateDto
    {
        public int OfficerId { get; set; }           
        public string OfficerName { get; set; } = ""; 
        public int ZoneId { get; set; }               
    }
}