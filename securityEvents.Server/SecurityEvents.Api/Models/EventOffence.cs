using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SecurityEvents.Api.Models;

[Keyless]
[Table("event_offence", Schema = "dbo")]
public partial class EventOffence
{
    [Column("event_offence")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? EventOffence1 { get; set; }
}
