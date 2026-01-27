using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SecurityEvents.Api.Models;

[Keyless]
[Table("T_AS400_Company_tmp", Schema = "dbo")]
public partial class TAs400CompanyTmp
{
    [Column("AC_Hevra_ID")]
    public int AcHevraId { get; set; }

    [Column("AC_Hevra_Name")]
    [StringLength(100)]
    [Unicode(false)]
    public string? AcHevraName { get; set; }

    [Column("AC_Short_Name")]
    [StringLength(100)]
    [Unicode(false)]
    public string? AcShortName { get; set; }

    [Column("AC_Updated")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AcUpdated { get; set; }
}
