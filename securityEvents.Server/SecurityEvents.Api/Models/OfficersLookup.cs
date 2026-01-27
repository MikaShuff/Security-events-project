using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SecurityEvents.Api.Models;

[Table("officers_lookup", Schema = "dbo")]
[Index("OfficerName", Name = "IX_officers_lookup_officer_name")]
[Index("ZoneId", Name = "IX_officers_lookup_zone_id")]
public partial class OfficersLookup
{
    [Key]
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
    [StringLength(20)]
    [Unicode(false)]
    public string? OfficerTz { get; set; }

    [Column("officer_worker")]
    public int? OfficerWorker { get; set; }

    [Column("officer_role")]
    [StringLength(100)]
    [Unicode(false)]
    public string? OfficerRole { get; set; }

    [Column("zone_id")]
    public int? ZoneId { get; set; }
}
