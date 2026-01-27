using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SecurityEvents.Api.Models;

[Table("zone", Schema = "dbo")]
public partial class Zone
{
    [Key]
    [Column("zone_id")]
    public int ZoneId { get; set; }

    [Column("zone_name")]
    [StringLength(100)]
    [Unicode(false)]
    public string ZoneName { get; set; } = null!;

    [InverseProperty("Zone")]
    public virtual ICollection<Officer> Officers { get; set; } = new List<Officer>();

    [ForeignKey("ZoneId")]
    [InverseProperty("Zones")]
    public virtual ICollection<TAs400Branch> AbSnifs { get; set; } = new List<TAs400Branch>();
}
