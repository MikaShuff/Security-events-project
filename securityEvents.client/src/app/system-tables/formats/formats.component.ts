import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { API_BASE } from '../../config';

const API = `${API_BASE}/api/formats`;

type Format = {
  id: number;
  name: string;
  shortName?: string;
  updated?: string;
  _edit?: boolean;
};

@Component({
  selector: 'app-formats',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './formats.component.html',
  styleUrls: ['./formats.component.css'],
})
export class FormatsComponent implements OnInit {

  constructor(private http: HttpClient) {}

  data: Format[] = [];

  // 🔥 סינון לייב
  filter = {
    id: '',
    name: '',
    short: ''
  };

  get filtered(): Format[] {
    const id    = (this.filter.id    ?? '').trim();
    const name  = (this.filter.name  ?? '').trim().toLowerCase();
    const short = (this.filter.short ?? '').trim().toLowerCase();

    return this.data.filter(r =>
      (!id    || String(r.id).includes(id)) &&
      (!name  || (r.name       || '').toLowerCase().includes(name)) &&
      (!short || (r.shortName  || '').toLowerCase().includes(short))
    );
  }

  ngOnInit() { this.loadData(); }

  loadData() {
    this.http.get<Format[]>(API)
      .subscribe(res => this.data = res);
  }

  onAdd() {
    const newRow: Format = {
      id: 0,
      name: '',
      shortName: '',
      updated: new Date().toISOString().slice(0, 10),
      _edit: true
    };

    this.data = [newRow, ...this.data];
  }

  startEdit(row: Format) { row._edit = true; }

  cancelEdit(row: Format) {
    if (row.id === 0) {
      this.data = this.data.filter(r => r !== row);
      return;
    }
    row._edit = false;
  }

  saveRow(row: Format) {
    const body = {
      acHevraName: row.name,
      acShortName: row.shortName,
      acUpdated: row.updated
    };

    const request$ =
      row.id === 0
        ? this.http.post<Format>(API, body)
        : this.http.put<Format>(`${API}/${row.id}`, body);

    request$.subscribe({
      next: () => this.loadData(),
      error: err => console.error('שמירה נכשלה', err)
    });

    row._edit = false;
  }

  onDelete(row: Format) {
    if (!confirm(`למחוק את "${row.name}"?`)) return;
    this.http.delete(`${API}/${row.id}`)
      .subscribe(() => this.loadData());
  }

  trackById(_: number, row: Format) { return row.id; }
}