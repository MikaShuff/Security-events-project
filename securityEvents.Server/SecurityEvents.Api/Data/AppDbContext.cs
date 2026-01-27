//AppDbContext.cs

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SecurityEvents.Api.Models;

namespace SecurityEvents.Api.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventOffence> EventOffences { get; set; }

    public virtual DbSet<Events2025> Events2025s { get; set; }

    public virtual DbSet<EventsType> EventsTypes { get; set; }

    public virtual DbSet<Handling> Handlings { get; set; }

    public virtual DbSet<Offence> Offences { get; set; }

    public virtual DbSet<Officer> Officers { get; set; }

    public virtual DbSet<Officers2025> Officers2025s { get; set; }

    public virtual DbSet<OfficersExt> OfficersExts { get; set; }

    public virtual DbSet<OfficersExt2025> OfficersExt2025s { get; set; }

    public virtual DbSet<OfficersLookup> OfficersLookups { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<SubEventsType> SubEventsTypes { get; set; }

    public virtual DbSet<TAs400Branch> TAs400Branches { get; set; }

    public virtual DbSet<TAs400Branches2025> TAs400Branches2025s { get; set; }

    public virtual DbSet<TAs400BranchesTmp> TAs400BranchesTmps { get; set; }

    public virtual DbSet<TAs400Company> TAs400Companies { get; set; }

    public virtual DbSet<TAs400CompanyTmp> TAs400CompanyTmps { get; set; }

    public virtual DbSet<TAs400Eshkol> TAs400Eshkols { get; set; }

    public virtual DbSet<TAs400EshkolTmp> TAs400EshkolTmps { get; set; }

    public virtual DbSet<Zone> Zones { get; set; }

    public virtual DbSet<ZonesBranchs2025> ZonesBranchs2025s { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>(entity =>
        {
            entity.Property(e => e.CeoRemark).UseCollation("SQL_Latin1_General_CP1255_CI_AS");
            entity.Property(e => e.EventDesc).UseCollation("SQL_Latin1_General_CP1255_CI_AS");
            entity.Property(e => e.EventId).ValueGeneratedOnAdd();
            entity.Property(e => e.HandleDesc).UseCollation("SQL_Latin1_General_CP1255_CI_AS");
            entity.Property(e => e.Remark).UseCollation("SQL_Latin1_General_CP1255_CI_AS");

            entity.HasOne(d => d.Officer).WithMany()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_events_officer");
        });

        modelBuilder.Entity<EventOffence>(entity =>
        {
            entity.Property(e => e.EventOffence1).UseCollation("SQL_Latin1_General_CP1255_CI_AS");
        });

        modelBuilder.Entity<Events2025>(entity =>
        {
            entity.Property(e => e.CeoRemark).UseCollation("SQL_Latin1_General_CP1255_CI_AS");
            entity.Property(e => e.EventDesc).UseCollation("SQL_Latin1_General_CP1255_CI_AS");
            entity.Property(e => e.EventId).ValueGeneratedOnAdd();
            entity.Property(e => e.HandleDesc).UseCollation("SQL_Latin1_General_CP1255_CI_AS");
            entity.Property(e => e.Remark).UseCollation("SQL_Latin1_General_CP1255_CI_AS");
        });

        modelBuilder.Entity<EventsType>(entity =>
        {
            entity.HasKey(e => e.EventType).HasName("PK_events");

            entity.Property(e => e.EventType).ValueGeneratedNever();
            entity.Property(e => e.EventName).UseCollation("SQL_Latin1_General_CP1255_CI_AS");
        });

        modelBuilder.Entity<Handling>(entity =>
        {
            entity.HasKey(e => e.HandlingType).IsClustered(false);

            entity.Property(e => e.HandlingType).ValueGeneratedNever();
            entity.Property(e => e.HandlingName).UseCollation("SQL_Latin1_General_CP1255_CI_AS");
        });

        modelBuilder.Entity<Officer>(entity =>
        {
            entity.Property(e => e.OfficerId).ValueGeneratedNever();
            entity.Property(e => e.OfficerName).UseCollation("SQL_Latin1_General_CP1255_CI_AS");

            entity.HasOne(d => d.Zone).WithMany(p => p.Officers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_officers_zone");
        });

        modelBuilder.Entity<Officers2025>(entity =>
        {
            entity.Property(e => e.OfficerName).UseCollation("SQL_Latin1_General_CP1255_CI_AS");
        });

        modelBuilder.Entity<OfficersExt>(entity =>
        {
            entity.HasKey(e => e.OfficerId).HasName("PK_officers_Ext");

            entity.Property(e => e.OfficerId).ValueGeneratedNever();
        });

        modelBuilder.Entity<OfficersLookup>(entity =>
        {
            entity.HasKey(e => e.OfficerId).HasName("PK__officers__AF7899976DB29ED7");

            entity.Property(e => e.OfficerId).ValueGeneratedNever();
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.Property(e => e.StatusId).ValueGeneratedNever();
            entity.Property(e => e.StatusDescription).UseCollation("SQL_Latin1_General_CP1255_CI_AS");
        });

        modelBuilder.Entity<SubEventsType>(entity =>
        {
            entity.HasKey(e => new { e.SubEventId, e.EventType }).HasName("PK_sub_events");

            entity.Property(e => e.SubEventName).UseCollation("SQL_Latin1_General_CP1255_CI_AS");

            entity.HasOne(d => d.EventTypeNavigation).WithMany(p => p.SubEventsTypes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_sub_events_events");
        });

        modelBuilder.Entity<TAs400Branch>(entity =>
        {
            entity.Property(e => e.AbSnifId).ValueGeneratedNever();
            entity.Property(e => e.AbSnifName).UseCollation("SQL_Latin1_General_CP1255_CI_AS");
            entity.Property(e => e.AbUpdated).UseCollation("SQL_Latin1_General_CP1255_CI_AS");
        });

        modelBuilder.Entity<TAs400Branches2025>(entity =>
        {
            entity.Property(e => e.AbSnifName).UseCollation("SQL_Latin1_General_CP1255_CI_AS");
            entity.Property(e => e.AbUpdated).UseCollation("SQL_Latin1_General_CP1255_CI_AS");
        });

        modelBuilder.Entity<TAs400BranchesTmp>(entity =>
        {
            entity.Property(e => e.AbSnifName).UseCollation("SQL_Latin1_General_CP1255_CI_AS");
            entity.Property(e => e.AbUpdated).UseCollation("SQL_Latin1_General_CP1255_CI_AS");
        });

        modelBuilder.Entity<TAs400Company>(entity =>
        {
            entity.Property(e => e.AcHevraId).ValueGeneratedNever();
            entity.Property(e => e.AcHevraName).UseCollation("SQL_Latin1_General_CP1255_CI_AS");
            entity.Property(e => e.AcShortName).UseCollation("SQL_Latin1_General_CP1255_CI_AS");
            entity.Property(e => e.AcUpdated).UseCollation("SQL_Latin1_General_CP1255_CI_AS");
        });

        modelBuilder.Entity<TAs400CompanyTmp>(entity =>
        {
            entity.Property(e => e.AcHevraName).UseCollation("SQL_Latin1_General_CP1255_CI_AS");
            entity.Property(e => e.AcShortName).UseCollation("SQL_Latin1_General_CP1255_CI_AS");
            entity.Property(e => e.AcUpdated).UseCollation("SQL_Latin1_General_CP1255_CI_AS");
        });

        modelBuilder.Entity<TAs400Eshkol>(entity =>
        {
            entity.Property(e => e.AeEshkolManage).UseCollation("SQL_Latin1_General_CP1255_CI_AS");
            entity.Property(e => e.AeEshkolName).UseCollation("SQL_Latin1_General_CP1255_CI_AS");
            entity.Property(e => e.AeUpdated).UseCollation("SQL_Latin1_General_CP1255_CI_AS");
        });

        modelBuilder.Entity<TAs400EshkolTmp>(entity =>
        {
            entity.Property(e => e.AeEshkolManage).UseCollation("SQL_Latin1_General_CP1255_CI_AS");
            entity.Property(e => e.AeEshkolName).UseCollation("SQL_Latin1_General_CP1255_CI_AS");
            entity.Property(e => e.AeUpdated).UseCollation("SQL_Latin1_General_CP1255_CI_AS");
        });

        modelBuilder.Entity<Zone>(entity =>
        {
            entity.Property(e => e.ZoneId).ValueGeneratedNever();
            entity.Property(e => e.ZoneName).UseCollation("SQL_Latin1_General_CP1255_CI_AS");

            entity.HasMany(d => d.AbSnifs).WithMany(p => p.Zones)
                .UsingEntity<Dictionary<string, object>>(
                    "ZonesBranch",
                    r => r.HasOne<TAs400Branch>().WithMany()
                        .HasForeignKey("AbSnifId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_zones_branchs_T_AS400_Branches"),
                    l => l.HasOne<Zone>().WithMany()
                        .HasForeignKey("ZoneId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_zones_branchs_zone"),
                    j =>
                    {
                        j.HasKey("ZoneId", "AbSnifId");
                        j.ToTable("zones_branchs", "dbo");
                        j.IndexerProperty<int>("ZoneId").HasColumnName("zone_id");
                        j.IndexerProperty<int>("AbSnifId").HasColumnName("ab_snif_id");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
