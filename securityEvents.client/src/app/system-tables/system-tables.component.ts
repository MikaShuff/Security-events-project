// system-tables.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, RouterOutlet } from '@angular/router';

type SystemTable = {
  name: string;
  description?: string;
  rows: number;
  updatedAt: Date | string;
};

@Component({
  selector: 'app-system-tables',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    RouterModule   // ⭐ חובה כדי ש-routerLink יעבוד!
  ],
  templateUrl: './system-tables.component.html',
  styleUrls: ['./system-tables.component.css'],
})
export class SystemTablesComponent implements OnInit {

  loading = true;
  error: string | null = null;

  tables: SystemTable[] = [];

  async ngOnInit() {
    try {
      this.tables = [
        { name: 'Users', description: 'משתמשים', rows: 1245, updatedAt: new Date() },
        { name: 'Roles', description: 'תפקידים', rows: 18, updatedAt: new Date() },
        { name: 'AuditLog', description: 'יומן פעולות', rows: 98765, updatedAt: new Date() },
      ];
    } catch (e: any) {
      this.error = e?.message ?? 'שגיאה בטעינת טבלאות';
    }

    this.loading = false;
  }
}
