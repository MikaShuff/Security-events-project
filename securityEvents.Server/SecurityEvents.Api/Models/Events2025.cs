using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SecurityEvents.Api.Models;

[Keyless]
[Table("events_2025", Schema = "dbo")]
public partial class Events2025
{
    [Column("event_id")]
    public int EventId { get; set; }

    [Column("date_modified", TypeName = "datetime")]
    public DateTime DateModified { get; set; }

    [Column("event_type")]
    public int EventType { get; set; }

    [Column("event_desc")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? EventDesc { get; set; }

    [Column("sub_event_id")]
    public int SubEventId { get; set; }

    [Column("branch_num")]
    public int BranchNum { get; set; }

    [Column("event_sum", TypeName = "decimal(18, 0)")]
    public decimal EventSum { get; set; }

    [Column("handle_type")]
    public int HandleType { get; set; }

    [Column("handle_desc")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? HandleDesc { get; set; }

    [Column("officer_id")]
    public int OfficerId { get; set; }

    [Column("remark")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? Remark { get; set; }

    [Column("status_id")]
    public int StatusId { get; set; }

    [Column("event_date", TypeName = "datetime")]
    public DateTime EventDate { get; set; }

    [Column("ceo_remark")]
    [StringLength(1000)]
    [Unicode(false)]
    public string? CeoRemark { get; set; }

    [Column("customer_tz")]
    [StringLength(20)]
    [Unicode(false)]
    public string? CustomerTz { get; set; }
}
