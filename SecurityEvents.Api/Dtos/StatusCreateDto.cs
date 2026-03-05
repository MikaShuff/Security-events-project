namespace SecurityEvents.Api.Dtos
{
    public class StatusCreateDto
    {
        public int StatusId { get; set; }              // חובה (ValueGeneratedNever)
        public string StatusDescription { get; set; } = "";
    }
}
