using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SecurityEvents.Api.Models;

[Keyless]
[Table("officersExt_2025", Schema = "dbo")]
public partial class OfficersExt2025
{
    [Column("officer_id")]
    public int OfficerId { get; set; }

    [Column("officer_name")]
    [StringLength(50)]
    [Unicode(false)]
    public string? OfficerName { get; set; }

    [Column("officer_name_alt")]
    [StringLength(50)]
    [Unicode(false)]
    public string? OfficerNameAlt { get; set; }

    [Column("officer_tz")]
    public int? OfficerTz { get; set; }

    [Column("officer_worker")]
    public int? OfficerWorker { get; set; }

    [Column("officer_role")]
    [StringLength(100)]
    [Unicode(false)]
    public string? OfficerRole { get; set; }
}
