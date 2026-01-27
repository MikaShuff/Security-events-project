//Handling.cs

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SecurityEvents.Api.Models;

[Table("handling", Schema = "dbo")]
public partial class Handling
{
    [Key]
    [Column("handling_type")]
    public int HandlingType { get; set; }

    [Column("handling_name")]
    [StringLength(100)]
    [Unicode(false)]
    public string HandlingName { get; set; } = null!;
}
