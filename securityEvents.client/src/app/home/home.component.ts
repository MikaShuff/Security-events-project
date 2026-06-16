//home.component.ts - דף הבית עם סטטיסטיקות ואירועים אחרונים

import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { API_BASE } from '../config';

type RecentEventVm = {
  id: number;
  title: string;
  date: string;
  status: string;
};

@Component({
  standalone: true,
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
  imports: [CommonModule, RouterLink],
})
export class HomeComponent implements OnInit {
  loading = true;
  error = '';

  recentEvents: RecentEventVm[] = [];

  stats = {
    total: 0,
    open: 0,
    today: 0,
    week: 0,
  };

  //  סטטוס סגור"
  private CLOSED_STATUS_IDS = new Set<number>([2]);

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.loadDashboard();
  }

  private loadDashboard(): void {
    this.loading = true;
    this.error = '';

    // משתמשים ב-proxy: /api/... במקום localhost
    this.http
      .get<any>(`${API_BASE}/api/events?page=1&pageSize=50`)
      .subscribe({
        next: (res) => {
          const data = res?.data ?? res ?? [];
          const totalCount = res?.totalCount ?? res?.total ?? data.length;

          // Recent (5 אחרונים לפי תאריך)
          const sorted = [...data].sort((a: any, b: any) => {
            const da = new Date(a.eventDate ?? a.dateModified ?? a.date ?? 0).getTime();
            const db = new Date(b.eventDate ?? b.dateModified ?? b.date ?? 0).getTime();
            return db - da;
          });

          this.recentEvents = sorted.slice(0, 5).map((ev: any) => ({
            id: ev.eventId ?? ev.id,
            title: (ev.eventDesc ?? ev.name ?? '').trim() || `אירוע #${ev.eventId ?? ev.id}`,
            date: ev.eventDate ?? ev.dateModified ?? ev.date,
            status: this.mapStatus(ev.statusId),
          }));

          // Stats
          const now = new Date();
          const startOfToday = new Date(now.getFullYear(), now.getMonth(), now.getDate());
          const day = startOfToday.getDay(); // 0=Sunday
          const diffToSunday = day; 
          const startOfWeek = new Date(startOfToday);
          startOfWeek.setDate(startOfToday.getDate() - diffToSunday);

          const isToday = (d: Date) => d >= startOfToday;
          const isThisWeek = (d: Date) => d >= startOfWeek;

          let today = 0;
          let week = 0;
          let open = 0;

          for (const ev of data) {
            const dt = new Date(ev.eventDate ?? ev.dateModified ?? ev.date ?? 0);
            if (!isNaN(dt.getTime())) {
              if (isToday(dt)) today++;
              if (isThisWeek(dt)) week++;
            }

            const sid = Number(ev.statusId);
            if (!Number.isNaN(sid) && !this.CLOSED_STATUS_IDS.has(sid)) {
              open++;
            }
          }

          this.stats = {
            total: Number(totalCount) || data.length,
            open,
            today,
            week,
          };

          this.loading = false;
        },
        error: (err) => {
          console.error(err);
          this.error = 'לא הצלחתי לטעון נתונים לדף הבית';
          this.loading = false;
        },
      });
  }

  private mapStatus(statusId: any): string {
    const id = Number(statusId);
    if (Number.isNaN(id)) return '—';
    if (this.CLOSED_STATUS_IDS.has(id)) return 'סגור';
    return 'פתוח';
  }
}