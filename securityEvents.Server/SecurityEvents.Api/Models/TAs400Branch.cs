//TAs400Branch.cs

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SecurityEvents.Api.Models;

[Table("T_AS400_Branches", Schema = "dbo")]
public partial class TAs400Branch
{
    [Key]
    [Column("AB_Snif_ID")]
    public int AbSnifId { get; set; }

    [Column("AB_Snif_Name")]
    [StringLength(100)]
    [Unicode(false)]
    public string? AbSnifName { get; set; }

    [Column("AB_Reshet_ID")]
    public int? AbReshetId { get; set; }

    [Column("AB_Eshkol_ID")]
    public int? AbEshkolId { get; set; }

    [Column("AB_Updated")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AbUpdated { get; set; }

    [Column("AB_Update_ID")]
    public int? AbUpdateId { get; set; }

    [ForeignKey("AbSnifId")]
    [InverseProperty("AbSnifs")]
    public virtual ICollection<Zone> Zones { get; set; } = new List<Zone>();
}
