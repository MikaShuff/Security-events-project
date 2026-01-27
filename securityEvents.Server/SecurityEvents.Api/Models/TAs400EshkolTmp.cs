using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SecurityEvents.Api.Models;

[Keyless]
[Table("T_AS400_Eshkol_tmp", Schema = "dbo")]
public partial class TAs400EshkolTmp
{
    [Column("AE_Eshkol_ID")]
    public int AeEshkolId { get; set; }

    [Column("AE_Merhav")]
    public int? AeMerhav { get; set; }

    [Column("AE_Eshkol_Name")]
    [StringLength(50)]
    public string? AeEshkolName { get; set; }

    [Column("AE_Eshkol_Manage")]
    [StringLength(100)]
    [Unicode(false)]
    public string? AeEshkolManage { get; set; }

    [Column("AE_Updated")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AeUpdated { get; set; }
}
