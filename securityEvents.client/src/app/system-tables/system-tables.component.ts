// system-tables.component.ts
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';

type SystemTable = {
  name: string;
  description?: string;
  rows: number;
  updatedAt: Date | string;
};

@Component({
  selector: 'app-system-tables',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './system-tables.component.html',
  styleUrls: ['./system-tables.component.scss'],
})
export class SystemTablesComponent implements OnInit {
  loading = true;
  error: string | null = null;
  tables: SystemTable[] = [];

  async ngOnInit() {
    try {
      // כאן תקראי ל־API האמיתי שלכם
      this.tables = await Promise.resolve([
        { name: 'Users', description: 'משתמשים', rows: 1245, updatedAt: new Date() },
        { name: 'Roles', description: 'תפקידים', rows: 18, updatedAt: new Date() },
        { name: 'AuditLog', description: 'יומן פעולות', rows: 98765, updatedAt: new Date() },
      ]);
    } catch (e: any) {
      this.error = e?.message ?? 'שגיאה בטעינת טבלאות';
    } finally {
      this.loading = false;
    }
  }
}
