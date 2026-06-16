import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { API_BASE } from '../../config';

const API = `${API_BASE}/api/officers`;
const API_ADMIN = `${API_BASE}/api/officers/admin`; // מושך גם את שם האזור לכל קב"ט

type OfficerRow = {
  officerId: number;
  officerName: string;
  zoneName: string;    // ✅ במקום zoneId
  _edit?: boolean;
};

@Component({
  selector: 'app-officers',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './officers.component.html',
  styleUrls: ['./officers.component.css'],
})

export class OfficersComponent implements OnInit {

  constructor(private http: HttpClient) {}

  rows: OfficerRow[] = [];

  // ✅ סינון לייב
  filter = { id: '', name: '', zone: '' };

  get filteredRows(): OfficerRow[] {
    const id   = (this.filter.id   ?? '').trim();
    const name = (this.filter.name ?? '').trim().toLowerCase();
    const zone = (this.filter.zone ?? '').trim().toLowerCase();

    return this.rows.filter(r =>
      (!id   || String(r.officerId).includes(id)) &&
      (!name || (r.officerName || '').toLowerCase().includes(name)) &&
      (!zone || (r.zoneName || '').toLowerCase().includes(zone))
    );
  }

  ngOnInit() { this.load(); }

  load() {
  this.http.get<any[]>(API_ADMIN).subscribe(res => {
    this.rows = res.map(x => ({
      officerId: x.officerId,
      officerName: x.officerName,
      zoneName: x.zoneName
    }));
  });
}

  onAdd() {
    const newRow: OfficerRow = {
      officerId: 0,
      officerName: '',
      zoneName: '',
      _edit: true
    };

    this.rows = [newRow, ...this.rows];
  }

  startEdit(r: OfficerRow) { r._edit = true; }
  cancelEdit(r: OfficerRow) { r._edit = false; }

  saveRow(r: OfficerRow) {
    if (r.officerId === 0) {

      if (!r.zoneName.trim()) {
        alert('יש להזין ZoneName');
        return;
      }

      const newId = this.getNextId();
      const body = {
        officerId: newId,
        officerName: r.officerName,
        zoneName: r.zoneName
      };

      this.http.post(API, body).subscribe(() => this.load());
      r._edit = false;
      return;
    }

    const body: any = {
      officerName: r.officerName,
      zoneName: r.zoneName
    };

    this.http.put(`${API}/${r.officerId}`, body).subscribe(() => this.load());
    r._edit = false;
  }

  onDelete(r: OfficerRow) {
    if (!confirm(`למחוק את "${r.officerName}"?`)) return;
    this.http.delete(`${API}/${r.officerId}`).subscribe(() => this.load());
  }

  getNextId(): number {
    return Math.max(...this.rows.map(x => x.officerId), 0) + 1;
  }

  trackById = (_: number, r: OfficerRow) => r.officerId;
}