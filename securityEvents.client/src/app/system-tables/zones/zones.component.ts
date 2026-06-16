// src/app/system-tables/zones/zones.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { API_BASE } from '../../config'; 


type ZoneRow = { id: number; name: string };

@Component({
  selector: 'app-zones',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './zones.component.html',
  styleUrls: ['./zones.component.css'],
})
export class ZonesComponent implements OnInit {
  constructor(private http: HttpClient) {}

  rows: ZoneRow[] = [];

  // סינון פשוט
  filter = { id: '', name: '' };

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.http.get<ZoneRow[]>(`${API_BASE}/api/zones`).subscribe(res => this.rows = res);
  }

  get filtered(): ZoneRow[] {
    const id = this.filter.id.trim();
    const name = this.filter.name.trim().toLowerCase();
    return this.rows.filter(r =>
      (!id || String(r.id).includes(id)) &&
      (!name || r.name.toLowerCase().includes(name))
    );
  }

  trackById = (_: number, r: ZoneRow) => r.id;
}