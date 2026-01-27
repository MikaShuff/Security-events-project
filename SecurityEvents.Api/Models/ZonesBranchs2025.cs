using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SecurityEvents.Api.Models;

[Keyless]
[Table("zones_branchs_2025", Schema = "dbo")]
public partial class ZonesBranchs2025
{
    [Column("zone_id")]
    public int ZoneId { get; set; }

    [Column("ab_snif_id")]
    public int AbSnifId { get; set; }
}
