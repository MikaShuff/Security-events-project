import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { API_BASE } from '../../config';

// IIS Express אצלך
const API = `${API_BASE}/api/handlings`;

type HandlingRow = {
  id: number;           // HandlingType
  name: string;         // HandlingName
  _edit?: boolean;
};

@Component({
  selector: 'app-handlings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './handlings.component.html',
  styleUrls: ['./handlings.component.css'],
})
export class HandlingsComponent implements OnInit {

  constructor(private http: HttpClient) {}

  rows: HandlingRow[] = [];

  // 🔥 סינון LIVE – כמו בשאר המסכים (סניפים/אירועים/סוגי אירועים)
  filter = { id: '', name: '' };

  get filtered(): HandlingRow[] {
    const id   = (this.filter.id   ?? '').trim();
    const name = (this.filter.name ?? '').trim().toLowerCase();

    return this.rows.filter(r =>
      (!id   || String(r.id).includes(id)) &&
      (!name || (r.name || '').toLowerCase().includes(name))
    );
  }

  ngOnInit(): void { this.load(); }

  load(): void {
    this.http.get<HandlingRow[]>(API).subscribe(res => this.rows = res);
  }

  onAdd(): void {
    const newRow: HandlingRow = { id: 0, name: '', _edit: true };
    this.rows = [newRow, ...this.rows];
  }

  startEdit(r: HandlingRow) { r._edit = true; }
  cancelEdit(r: HandlingRow) { r._edit = false; }

  saveRow(r: HandlingRow): void {
    if (r.id === 0) {
      // POST — חייבים לבחור מזהה חדש (ValueGeneratedNever בשרת)
      const newId = this.getNextId();
      const body = { handlingType: newId, handlingName: r.name };

      this.http.post(API, body).subscribe({
        next: () => this.load(),
        error: err => alert(this.formatError(err))
      });

      r._edit = false;
      return;
    }

    // PUT — עדכון שם בלבד
    const body = { handlingName: r.name };
    this.http.put(`${API}/${r.id}`, body).subscribe({
      next: () => this.load(),
      error: err => alert(this.formatError(err))
    });

    r._edit = false;
  }

  onDelete(r: HandlingRow): void {
    if (!confirm(`למחוק את "${r.name}"?`)) return;
    this.http.delete(`${API}/${r.id}`).subscribe({
      next: () => this.load(),
      error: err => alert(this.formatError(err))
    });
  }

  getNextId(): number {
    return Math.max(...this.rows.map(x => x.id), 0) + 1;
  }

  trackById = (_: number, r: HandlingRow) => r.id;

  private formatError(err: any): string {
    return err?.error?.title || 'שגיאת שמירה/מחיקה. בדוק ערכים והאם המזהה כבר קיים.';
  }
}