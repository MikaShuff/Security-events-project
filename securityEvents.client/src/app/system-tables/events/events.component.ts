// src/app/system-tables/events/events.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { API_BASE } from '../../config';

const API = `${API_BASE}/api/Events`;

/* ========= SERVER TYPES ========= */

type ServerEvent = {
  eventId: number;
  eventDate: string;
  branchNum: number;
  eventType: number;
  subEventId: number;
  officerId: number;
  handleType: number;

  statusId: number;
  statusName: string;   // ✅ מגיע עכשיו מהשרת

  eventSum: number;
  eventDesc?: string;
  handleDesc?: string;
  remark?: string;
  ceoRemark?: string;
  customerTz?: string;
};

type PagedResponse = {
  data: ServerEvent[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
};

/* ========= UI TYPE ========= */

type UiEvent = {
  id: number;
  eventDateLocal: string;
  branchNum: number;
  eventType: number;
  subEventId: number;
  officerId: number;
  handleType: number;

  statusId: number;     // לשמירה (PUT / POST)
  statusName: string;   // ✅ לתצוגה בטבלה

  eventSum?: number;
  eventDesc: string;
  handleDesc?: string;
  remark?: string;
  ceoRemark?: string;
  customerTz: string;
  _edit?: boolean;
};

@Component({
  selector: 'app-events',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './‎events.component.html',
  styleUrls: ['./‎events.component.css'],
})
export class EventsComponent implements OnInit {

  constructor(private http: HttpClient) {}

  rows: UiEvent[] = [];
  page = 1;
  pageSize = 10;
  totalPages = 1;
  totalCount = 0;

  filter = {
    text: '',
    branchNum: '',
    eventType: '',
    statusId: ''
  };

  ngOnInit(): void {
    this.loadData();
  }

  /* ========= LOAD DATA ========= */

  loadData(): void {
    const params: string[] = [
      `page=${this.page}`,
      `pageSize=${this.pageSize}`
    ];

    if (this.filter.branchNum.trim())
      params.push(`branchNum=${+this.filter.branchNum}`);

    if (this.filter.eventType.trim())
      params.push(`eventType=${+this.filter.eventType}`);

    if (this.filter.statusId.trim())
      params.push(`statusId=${+this.filter.statusId}`);

    const url = `${API}?${params.join('&')}`;

    this.http.get<PagedResponse>(url).subscribe(res => {
      this.totalCount = res.totalCount;
      this.page = res.page;
      this.pageSize = res.pageSize;
      this.totalPages = res.totalPages;

      this.rows = res.data.map(e => this.toUi(e));
    });
  }

  /* ========= CLIENT FILTER ========= */

  get filteredRows(): UiEvent[] {
    const txt  = this.filter.text.trim().toLowerCase();
    const br   = this.filter.branchNum.trim();
    const type = this.filter.eventType.trim();
    const st   = this.filter.statusId.trim();

    return this.rows.filter(r =>
      (!txt ||
        r.eventDesc.toLowerCase().includes(txt) ||
        (r.handleDesc || '').toLowerCase().includes(txt) ||
        (r.remark || '').toLowerCase().includes(txt)
      ) &&
      (!br   || String(r.branchNum).includes(br)) &&
      (!type || String(r.eventType).includes(type)) &&
      (!st   || String(r.statusId).includes(st))
    );
  }

  /* ========= HELPERS ========= */

  private toLocalInputValue(dateIso: string): string {
    const d = new Date(dateIso);
    const p = (n: number) => String(n).padStart(2, '0');
    return `${d.getFullYear()}-${p(d.getMonth()+1)}-${p(d.getDate())}T${p(d.getHours())}:${p(d.getMinutes())}`;
  }

  private toUi(e: ServerEvent): UiEvent {
    return {
      id: e.eventId,
      eventDateLocal: this.toLocalInputValue(e.eventDate),
      branchNum: e.branchNum,
      eventType: e.eventType,
      subEventId: e.subEventId,
      officerId: e.officerId,
      handleType: e.handleType,

      statusId: e.statusId,
      statusName: e.statusName,       // ✅ כאן החיבור הנכון

      eventSum: e.eventSum,
      eventDesc: e.eventDesc ?? '',
      handleDesc: e.handleDesc ?? '',
      remark: e.remark ?? '',
      ceoRemark: e.ceoRemark ?? '',
      customerTz: e.customerTz ?? '',
      _edit: false
    };
  }

  /* ========= CRUD ========= */

  onAdd(): void {
    const nowLocal = this.toLocalInputValue(new Date().toISOString());

    const newRow: UiEvent = {
      id: 0,
      eventDateLocal: nowLocal,
      branchNum: 0,
      eventType: 0,
      subEventId: 0,
      officerId: 0,
      handleType: 0,

      statusId: 0,
      statusName: '',       // ✅ חובה כדי לא לשבור TypeScript

      eventSum: undefined,
      eventDesc: '',
      handleDesc: '',
      remark: '',
      ceoRemark: '',
      customerTz: '',
      _edit: true
    };

    this.rows = [newRow, ...this.rows];
  }

  startEdit(r: UiEvent) { r._edit = true; }
  cancelEdit(r: UiEvent) { r._edit = false; }

  saveRow(r: UiEvent): void {
    const eventDate = new Date(r.eventDateLocal);

    const body = {
      eventDate,
      branchNum: r.branchNum,
      eventType: r.eventType,
      subEventId: r.subEventId,
      officerId: r.officerId,
      handleType: r.handleType,
      statusId: r.statusId,
      eventSum: r.eventSum ?? null,
      eventDesc: r.eventDesc,
      handleDesc: r.handleDesc || null,
      remark: r.remark || null,
      ceoRemark: r.ceoRemark || null,
      customerTz: r.customerTz
    };

    if (r.id === 0) {
      this.http.post(API, body).subscribe(() => this.loadData());
    } else {
      this.http.put(`${API}/${r.id}`, body).subscribe(() => this.loadData());
    }

    r._edit = false;
  }

  onDelete(r: UiEvent): void {
    if (!confirm(`למחוק אירוע #${r.id}?`)) return;
    this.http.delete(`${API}/${r.id}`).subscribe(() => this.loadData());
  }

  /* ========= PAGING ========= */

  firstPage() { this.page = 1; this.loadData(); }
  prevPage()  { if (this.page > 1) { this.page--; this.loadData(); } }
  nextPage()  { if (this.page < this.totalPages) { this.page++; this.loadData(); } }
  lastPage()  { this.page = this.totalPages; this.loadData(); }

  trackById = (_: number, r: UiEvent) => r.id;
}
