//SubEventsType.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SecurityEvents.Api.Models;

[PrimaryKey("SubEventId", "EventType")]
[Table("sub_events_type", Schema = "dbo")]
public partial class SubEventsType
{
    [Key]
    [Column("sub_event_id")]
    public int SubEventId { get; set; }

    [Key]
    [Column("event_type")]
    public int EventType { get; set; }

    [Column("sub_event_name")]
    [StringLength(250)]
    [Unicode(false)]
    public string SubEventName { get; set; } = null!;

    [ForeignKey("EventType")]
    [InverseProperty("SubEventsTypes")]
    public virtual EventsType EventTypeNavigation { get; set; } = null!;
}
