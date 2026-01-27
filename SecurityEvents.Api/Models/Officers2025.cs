using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SecurityEvents.Api.Models;

[Keyless]
[Table("officers_2025", Schema = "dbo")]
public partial class Officers2025
{
    [Column("officer_id")]
    public int OfficerId { get; set; }

    [Column("officer_name")]
    [StringLength(50)]
    [Unicode(false)]
    public string OfficerName { get; set; } = null!;

    [Column("zone_id")]
    public int ZoneId { get; set; }
}
