// src/app/system-tables/sub-event-types/sub-event-types.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { API_BASE } from '../../config';

const API_SUB = `${API_BASE}/api/sub-event-types`;   // GET?eventType=..., POST, PUT/DELETE {eventType}/{subEventId}
const API_EVT = `${API_BASE}/api/event-types`;       // לטעינת סוגי אירועים ל-dropdown

type Lookup = { id: number; name: string };
type SubEventRow = { subEventId: number; name: string; _edit?: boolean };

@Component({
  selector: 'app-sub-event-types',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './sub-event-types.component.html',
  styleUrls: ['./sub-event-types.component.css'],
})
export class SubEventTypesComponent implements OnInit {

  constructor(private http: HttpClient) {}

  eventTypes: Lookup[] = [];
  selectedEventType: number | null = null;

  rows: SubEventRow[] = [];

  ngOnInit(): void {
    this.loadEventTypes();
  }

  // טוען סוגי אירועים ל-dropdown
  loadEventTypes(): void {
    this.http.get<Lookup[]>(API_EVT).subscribe(list => {
      this.eventTypes = list;
      // אם אין בחירה, בחר את הראשון
      if (!this.selectedEventType && this.eventTypes.length) {
        this.selectedEventType = this.eventTypes[0].id;
      }
      this.loadSubEvents();
    });
  }

  // טוען תתי-אירועים לפי סוג האירוע הנבחר
  loadSubEvents(): void {
    if (!this.selectedEventType) { this.rows = []; return; }
    this.http.get<Lookup[]>(`${API_SUB}?eventType=${this.selectedEventType}`)
      .subscribe(res => {
        this.rows = res.map(x => ({ subEventId: x.id, name: x.name }));
      });
  }

  // הוספה: שורה ריקה לעריכה
  onAdd(): void {
    if (!this.selectedEventType) { alert('בחר סוג אירוע לפני הוספה'); return; }
    this.rows = [{ subEventId: 0, name: '', _edit: true }, ...this.rows];
  }

  startEdit(r: SubEventRow) { r._edit = true; }
  cancelEdit(r: SubEventRow) { r._edit = false; }

  // שמירה: POST אם חדש, אחרת PUT
  saveRow(r: SubEventRow): void {
    if (!this.selectedEventType) { alert('בחר סוג אירוע'); return; }

    if (r.subEventId === 0) {
      // POST – חייבים גם eventType וגם subEventId (מפתח מורכב)
      const newId = this.getNextId();
      const body = {
        eventType: this.selectedEventType,
        subEventId: newId,
        subEventName: r.name
      };
      this.http.post(API_SUB, body).subscribe({
        next: () => this.loadSubEvents(),
        error: err => alert(this.errMsg(err))
      });
      r._edit = false;
      return;
    }

    // PUT – עדכון שם בלבד
    const body = { subEventName: r.name };
    this.http.put(`${API_SUB}/${this.selectedEventType}/${r.subEventId}`, body).subscribe({
      next: () => this.loadSubEvents(),
      error: err => alert(this.errMsg(err))
    });
    r._edit = false;
  }

  // מחיקה
  onDelete(r: SubEventRow): void {
    if (!this.selectedEventType) { alert('בחר סוג אירוע'); return; }
    if (!confirm(`למחוק את תת־אירוע #${r.subEventId}?`)) return;

    this.http.delete(`${API_SUB}/${this.selectedEventType}/${r.subEventId}`)
      .subscribe({
        next: () => this.loadSubEvents(),
        error: err => alert(this.errMsg(err))
      });
  }

  // יצירת מזהה תת־אירוע הבא, מתוך הרשימה הנוכחית
  getNextId(): number {
    return Math.max(...this.rows.map(x => x.subEventId), 0) + 1;
  }

  trackById = (_: number, r: SubEventRow) => `${r.subEventId}`;

  private errMsg(err: any): string {
    return err?.error?.title || err?.error || 'שגיאה: בדוק ערכים וכפילויות מפתח (eventType + subEventId).';
  }
}