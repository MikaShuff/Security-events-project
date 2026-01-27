using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SecurityEvents.Api.Models;

[Table("officers", Schema = "dbo")]
public partial class Officer
{
    [Key]
    [Column("officer_id")]
    public int OfficerId { get; set; }

    [Column("officer_name")]
    [StringLength(50)]
    [Unicode(false)]
    public string OfficerName { get; set; } = null!;

    [Column("zone_id")]
    public int ZoneId { get; set; }

    [ForeignKey("ZoneId")]
    [InverseProperty("Officers")]
    public virtual Zone Zone { get; set; } = null!;
}
