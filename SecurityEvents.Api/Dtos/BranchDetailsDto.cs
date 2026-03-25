// Api/Dtos/BranchDetailsDto.cs
namespace SecurityEvents.Api.Dtos;

public class BranchDetailsDto
{
    public int BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;

    public int? CompanyId { get; set; }                  // AC_Hevra_ID
    public string? CompanyName { get; set; }             // AC_Hevra_Name
    public string? CompanyShort { get; set; }            // AC_Short_Name

    public int? EshkolId { get; set; }                   // AE_Eshkol_ID
    public string? EshkolName { get; set; }              // AE_Eshkol_Name

    public int? SecurityZoneId { get; set; }             // zone_id
    public string? SecurityZoneName { get; set; }        // zone_name
}