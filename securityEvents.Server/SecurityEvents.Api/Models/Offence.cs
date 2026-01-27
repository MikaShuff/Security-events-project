using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SecurityEvents.Api.Models;

[Keyless]
[Table("offence", Schema = "dbo")]
public partial class Offence
{
    [Column("event_offence")]
    [StringLength(400)]
    public string EventOffence { get; set; } = null!;
}
