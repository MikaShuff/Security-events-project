using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SecurityEvents.Api.Models;

[Keyless]
[Table("T_AS400_Branches_2025", Schema = "dbo")]
public partial class TAs400Branches2025
{
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
}
