//Status.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SecurityEvents.Api.Models;

[Table("status", Schema = "dbo")]
public partial class Status
{
    [Key]
    [Column("status_id")]
    public int StatusId { get; set; }

    [Column("status_description")]
    [StringLength(250)]
    [Unicode(false)]
    public string StatusDescription { get; set; } = null!;
}
