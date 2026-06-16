import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { API_BASE } from '../../config';

type Branch = {
  id: number;
  name: string;
  _edit?: boolean;
};

@Component({
  selector: 'app-branches',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './branches.component.html',
  styleUrls: ['./branches.component.css'],
})
export class BranchesComponent implements OnInit {

  constructor(private http: HttpClient) {}

  data: Branch[] = [];

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.http.get<Branch[]>(`${API_BASE}/api/branches`)
      .subscribe(res => this.data = res);
  }

  // Live Filter - כמו אירועים
  filter = { id: '', name: '' };

 get filtered() {
  const id = (this.filter.id ?? '').trim();
  const name = (this.filter.name ?? '').trim();

  return this.data.filter(r =>
    (!id || String(r.id).includes(id)) &&
    (!name || r.name.includes(name))
  );
}


  // הוספת שורה חדשה
  onAdd() {
    const newRow: Branch = { id: 0, name: '', _edit: true };
    this.data = [newRow, ...this.data];
  }

  startEdit(r: Branch) { r._edit = true; }
  cancelEdit(r: Branch) { r._edit = false; }

  saveRow(r: Branch) {

    if (r.id === 0) {
      const body = {
        abSnifId: this.getNextId(),
        abSnifName: r.name,
        abReshetId: null,
        abEshkolId: null,
        abUpdated: null,
        abUpdateId: null
      };

      this.http.post(`${API_BASE}/api/branches`, body)
        .subscribe(() => this.loadData());

    } else {
      const body = { abSnifName: r.name };

      this.http.put(`${API_BASE}/api/branches/${r.id}`, body)
        .subscribe(() => this.loadData());
    }

    r._edit = false;
  }

  onDelete(r: Branch) {
    if (!confirm(`למחוק את הסניף "${r.name}"?`)) return;

    this.http.delete(`${API_BASE}/api/branches/${r.id}`)
      .subscribe(() => this.loadData());
  }

  getNextId(): number {
    return Math.max(...this.data.map(x => x.id), 0) + 1;
  }

  trackById(_: number, r: Branch) { return r.id; }
}