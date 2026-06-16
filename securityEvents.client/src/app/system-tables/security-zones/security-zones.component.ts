import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

type SecurityZone = {
  id: number;      // מספר אזור ביטחון
  name: string;    // שם אזור ביטחון
  _edit?: boolean;
};

@Component({
  selector: 'app-security-zones',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './security-zones.component.html',
  styleUrl: './security-zones.component.css'
})
export class SecurityZonesComponent {

  data: SecurityZone[] = [
    { id: 1, name: 'שכונת הקריות' },
    { id: 2, name: 'שכונת עקיבא' },
    { id: 3, name: 'אבן יעקב' },
    { id: 4, name: 'אזור תעשייה ניר' },
    { id: 5, name: 'בית הורד' },
    { id: 6, name: 'הולילנד' },
    { id: 7, name: 'אחוזת שמש' },
    { id: 8, name: 'שער השלושה / רחבעם' },
    { id: 9, name: 'קרית המדע' },
    { id: 10, name: 'שמורת הזיתים' }
  ];

  filter = { id: '', name: '' };

  get filtered(): SecurityZone[] {
    const id = this.filter.id.trim();
    const name = this.filter.name.trim();
    return this.data.filter(z =>
      (!id || String(z.id).includes(id)) &&
      (!name || z.name.includes(name))
    );
  }

  sort: { key: keyof SecurityZone | ''; dir: 1 | -1 } = { key: '', dir: 1 };

  sortBy(key: keyof SecurityZone) {
    if (this.sort.key === key) this.sort.dir = (this.sort.dir * -1) as 1 | -1;
    else { this.sort.key = key; this.sort.dir = 1; }
    const { dir } = this.sort;
    this.data = [...this.data].sort((a, b) =>
      ((a[key]! > b[key]!) ? 1 : (a[key]! < b[key]! ? -1 : 0)) * dir
    );
  }

  onAdd() {
    const id = (this.data.at(-1)?.id ?? 0) + 1;
    this.data = [{ id, name: 'חדש', _edit: true }, ...this.data];
  }

  startEdit(r: SecurityZone) { r._edit = true; }
  cancelEdit(r: SecurityZone) { r._edit = false; }
  saveRow(r: SecurityZone) { r._edit = false; }

  onDelete(r: SecurityZone) {
    if (!confirm(`למחוק את "${r.name}"?`)) return;
    this.data = this.data.filter(x => x.id !== r.id);
  }

  trackById = (_: number, r: SecurityZone) => r.id;
}
