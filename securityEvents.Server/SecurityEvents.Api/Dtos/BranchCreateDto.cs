namespace SecurityEvents.Api.Dtos
{
    public class BranchCreateDto
    {
        public int AbSnifId { get; set; }         // קוד סניף חדש
        public string AbSnifName { get; set; }    // שם סניף
        public int? AbReshetId { get; set; }
        public int? AbEshkolId { get; set; }
        public string? AbUpdated { get; set; }
        public int? AbUpdateId { get; set; }
    }
}