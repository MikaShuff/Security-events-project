import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { API_BASE } from '../../config';

type EventType = {
  id: number;
  name: string;
  _edit?: boolean;
};

@Component({
  selector: 'app-event-types',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './event-types.component.html',
  styleUrls: ['./event-types.component.css'],
})
export class EventTypesComponent implements OnInit {

  constructor(private http: HttpClient) {}

  data: EventType[] = [];

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.http.get<EventType[]>(`${API_BASE}/api/event-types`).subscribe(res => {
      this.data = res;
    });
  }

  // סינון (לא השתנה)
  filter = { id: '', name: '' };
  get filtered(): EventType[] {
    const id = this.filter.id.trim();
    const name = this.filter.name.trim();
    return this.data.filter(r =>
      (!id || String(r.id).includes(id)) &&
      (!name || r.name.includes(name))
    );
  }

  // מיון (לא השתנה)
  sort: { key: keyof EventType | ''; dir: 1 | -1 } = { key: '', dir: 1 };
  sortBy(key: keyof EventType) {
    if (this.sort.key === key) this.sort.dir = (this.sort.dir * -1) as 1 | -1;
    else { this.sort.key = key; this.sort.dir = 1; }
    const { dir } = this.sort;
    this.data = [...this.data].sort((a, b) => {
      const av = a[key] as any, bv = b[key] as any;
      return (av > bv ? 1 : av < bv ? -1 : 0) * dir;
    });
  }

  // הוספה חדשה
  onAdd() {
    const newItem = { id: 0, name: '', _edit: true };
    this.data = [newItem, ...this.data];
  }

  // עריכה
  startEdit(r: EventType) { r._edit = true; }
  cancelEdit(r: EventType) { r._edit = false; }

  // שמירה לשרת
  saveRow(r: EventType) {
    if (r.id === 0) {
      // זה POST
      const body = { eventType: this.getNextId(), eventName: r.name };
     this.http.post(`${API_BASE}/api/event-types`, body).subscribe(() => this.loadData());
    } else {
      // זה PUT
      const body = { eventName: r.name };
      this.http.put(`${API_BASE}/api/event-types/${r.id}`, body).subscribe(() => this.loadData());
    }

    r._edit = false;
  }

  // מחיקה לשרת
  onDelete(r: EventType) {
    if (!confirm(`למחוק את "${r.name}"?`)) return;
    this.http.delete(`${API_BASE}/api/event-types/${r.id}`).subscribe(() => this.loadData());
  }

  // הפקת ID חדש רק לצורך POST
  getNextId(): number {
    return (Math.max(...this.data.map(x => x.id), 0) + 1);
  }

  trackById = (_: number, r: EventType) => r.id;
}