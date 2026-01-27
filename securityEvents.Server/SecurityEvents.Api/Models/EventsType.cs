//EventsType.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SecurityEvents.Api.Models;

[Table("events_type", Schema = "dbo")]
public partial class EventsType
{
    [Key]
    [Column("event_type")]
    public int EventType { get; set; }

    [Column("event_name")]
    [StringLength(100)]
    [Unicode(false)]
    public string EventName { get; set; } = null!;

    [InverseProperty("EventTypeNavigation")]
    public virtual ICollection<SubEventsType> SubEventsTypes { get; set; } = new List<SubEventsType>();
}
