import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { API_BASE } from '../../config';

const API = `${API_BASE}/api/statuses`;

type StatusRow = {
  id: number;
  name: string;
  _edit?: boolean;
};

@Component({
  selector: 'app-statuses',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './statuses.component.html',
  styleUrls: ['./statuses.component.css'],
})
export class StatusesComponent implements OnInit {

  constructor(private http: HttpClient) {}

  rows: StatusRow[] = [];

  // 🔥 סינון לייב
  filter = { id: '', name: '' };

  get filtered(): StatusRow[] {
    const id   = (this.filter.id   ?? '').trim();
    const name = (this.filter.name ?? '').trim().toLowerCase();

    return this.rows.filter(r =>
      (!id   || String(r.id).includes(id)) &&
      (!name || (r.name || '').toLowerCase().includes(name))
    );
  }

  ngOnInit() {
    this.load();
  }

  load() {
    this.http.get<StatusRow[]>(API)
      .subscribe(res => this.rows = res);
  }

  onAdd() {
    const newRow: StatusRow = { id: 0, name: '', _edit: true };
    this.rows = [newRow, ...this.rows];
  }

  startEdit(r: StatusRow) { r._edit = true; }
  cancelEdit(r: StatusRow) { r._edit = false; }

  saveRow(r: StatusRow) {
    if (r.id === 0) {
      const newId = this.getNextId();

      const body = {
        statusId: newId,
        statusDescription: r.name
      };

      this.http.post(API, body)
        .subscribe(() => this.load());

      r._edit = false;
      return;
    }

    const body = { statusDescription: r.name };

    this.http.put(`${API}/${r.id}`, body)
      .subscribe(() => this.load());

    r._edit = false;
  }

  onDelete(r: StatusRow) {
    if (!confirm(`למחוק את "${r.name}"?`)) return;

    this.http.delete(`${API}/${r.id}`)
      .subscribe(() => this.load());
  }

  getNextId(): number {
    return Math.max(...this.rows.map(r => r.id), 0) + 1;
  }

  trackById(_: number, r: StatusRow) { return r.id; }
}
``