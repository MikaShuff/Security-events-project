//events-list.component.ts
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { EventsService } from '../events.service';
import { LookupsService } from '../lookups.service';
import { Event, EventQuery, LookupItem } from '../event.models';

@Component({
  selector: 'app-events-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './events-list.component.html',
  styleUrls: ['./events-list.component.css']
})
export class EventsListComponent implements OnInit {
  private eventsSrv = inject(EventsService);
  private lookupsSrv = inject(LookupsService);

  // נתונים להצגה
  events: Event[] = [];
  loading = false;
  error?: string;

  // עימוד
  currentPage = 1;
  pageSize = 20;
  totalCount = 0;
  totalPages = 0;

  // סינון (קשור ל-ngModel בטופס)
  filters: {
    fromDate?: string;
    toDate?: string;
    formatId?: number | null;
    branchId?: number | null;
    zoneId?: number | null;
    officerId?: number | null;
    eventTypeId?: number | null;
    subEventTypeId?: number | null;
    handlingId?: number | null;
    statusId?: number | null;
  } = {
      formatId: null,
      branchId: null,
      zoneId: null,
      officerId: null,
      eventTypeId: null,
      subEventTypeId: null,
      handlingId: null,
      statusId: null
    };

  // רשימות לוקאפים
  formats: LookupItem[] = [];
  branches: LookupItem[] = [];
  zones: LookupItem[] = [];
  officers: LookupItem[] = [];
  eventTypes: LookupItem[] = [];
  subEventTypes: LookupItem[] = [];
  handlings: LookupItem[] = [];
  statuses: LookupItem[] = [];

  // מפות id->name להצגה בטבלה (אם השרת לא מחזיר שמות)
  formatMap = new Map<number, string>();
  branchMap = new Map<number, string>();
  zoneMap = new Map<number, string>();
  officerMap = new Map<number, string>();
  eventTypeMap = new Map<number, string>();
  subEventTypeMap = new Map<number, string>();
  handlingMap = new Map<number, string>();
  statusMap = new Map<number, string>();

  ngOnInit(): void {
    this.loadLookups(); // נוריד לוקאפים במקביל
    this.reload();      // נטען אירועים (עם/בלי סינון)
  }

  private loadLookups(): void {
    // כל אחד בנפרד – פשוט להבנה; אפשר גם עם forkJoin בהמשך
    this.lookupsSrv.getFormats().subscribe(list => {
      this.formats = list;
      list.forEach(x => this.formatMap.set(x.id, x.name));
    });
    this.lookupsSrv.getBranches().subscribe(list => {
      this.branches = list;
      list.forEach(x => this.branchMap.set(x.id, x.name));
    });
    this.lookupsSrv.getZones().subscribe(list => {
      this.zones = list;
      list.forEach(x => this.zoneMap.set(x.id, x.name));
    });
    this.lookupsSrv.getOfficers().subscribe(list => {
      this.officers = list;
      list.forEach(x => this.officerMap.set(x.id, x.name));
    });
    this.lookupsSrv.getEventTypes().subscribe(list => {
      this.eventTypes = list;
      list.forEach(x => this.eventTypeMap.set(x.id, x.name));
    });
    this.lookupsSrv.getSubEventTypes().subscribe(list => {
      this.subEventTypes = list;
      list.forEach(x => this.subEventTypeMap.set(x.id, x.name));
    });
    this.lookupsSrv.getHandlings().subscribe(list => {
      this.handlings = list;
      list.forEach(x => this.handlingMap.set(x.id, x.name));
    });
    this.lookupsSrv.getStatuses().subscribe(list => {
      this.statuses = list;
      list.forEach(x => this.statusMap.set(x.id, x.name));
    });
  }

  // בניית אובייקט השאילתה לשרת
  private buildQuery(): EventQuery {
    const f = this.filters;
    return {
      fromDate: f.fromDate || undefined,
      toDate: f.toDate || undefined,
      formatId: f.formatId ?? undefined,
      branchId: f.branchId ?? undefined,
      zoneId: f.zoneId ?? undefined,
      officerId: f.officerId ?? undefined,
      eventTypeId: f.eventTypeId ?? undefined,
      subEventTypeId: f.subEventTypeId ?? undefined,
      handlingId: f.handlingId ?? undefined,
      statusId: f.statusId ?? undefined,
      page: this.currentPage,
      pageSize: this.pageSize
    };
  }

  reload(): void {
    this.loading = true;
    this.error = undefined;

    const query = this.buildQuery();

    this.eventsSrv.getAll(query).subscribe({
      next: (response) => {
        this.events = response.data;
        this.totalCount = response.totalCount;
        this.totalPages = response.totalPages;
        this.currentPage = response.page;
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        this.error = 'שגיאה בטעינת אירועים.';
        this.loading = false;
      }
    });
  }

  clearFilters(): void {
    this.filters = {
      formatId: null,
      branchId: null,
      zoneId: null,
      officerId: null,
      eventTypeId: null,
      subEventTypeId: null,
      handlingId: null,
      statusId: null
    };
    this.currentPage = 1; // חזרה לעמוד הראשון
    this.reload();
  }

  // פונקציות ניווט עימוד
  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
    this.reload();
  }

  nextPage(): void {
    this.goToPage(this.currentPage + 1);
  }

  previousPage(): void {
    this.goToPage(this.currentPage - 1);
  }

  // מערך עמודים להצגה (למשל: 1, 2, 3, ..., 10)
  get pageNumbers(): number[] {
    const pages: number[] = [];
    const maxPagesToShow = 5;
    let startPage = Math.max(1, this.currentPage - Math.floor(maxPagesToShow / 2));
    let endPage = Math.min(this.totalPages, startPage + maxPagesToShow - 1);

    if (endPage - startPage < maxPagesToShow - 1) {
      startPage = Math.max(1, endPage - maxPagesToShow + 1);
    }

    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }
    return pages;
  }

  // פונקציות עזר להצגת שם במקום מזהה
  nameFrom(map: Map<number, string>, id?: number | null): string {
    if (id === null || id === undefined) return '';
    return map.get(id) ?? String(id);
  }

}
