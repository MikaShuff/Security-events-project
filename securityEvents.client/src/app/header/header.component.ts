// src/app/core/header/header.component.ts
import { Component, HostListener, ElementRef, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { EventsService } from '../events/events.service';
import { AuthService, MeResponse } from '../auth/auth.service';
import { AuthState } from '../auth/auth-state.service';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
interface EventItem {
  id: number;
  name: string;
  date: string;
  owner: string;
}

@Component({
  standalone: true,
  selector: 'app-header',
  imports: [CommonModule, RouterLink],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css'],
})
export class HeaderComponent implements OnInit {
  me$!: Observable<MeResponse | null>;
  isLoggedIn$!: Observable<boolean>;

  eventsOpen = false;
  recentEvents: EventItem[] = [];
  loading = true;
  error = '';

  constructor(
    private el: ElementRef,
    private eventsService: EventsService,
    private router: Router,
    private auth: AuthService,
    private authState: AuthState,
  ) {}

  ngOnInit(): void {
  this.me$ = this.authState.me$;
  this.isLoggedIn$ = this.me$.pipe(map(me => !!me?.username));

  // load recent events only when logged in
  this.isLoggedIn$.subscribe((loggedIn) => {
    if (!loggedIn) {
      this.recentEvents = [];
      this.loading = false;
      this.error = '';
      return;
    }

    this.loading = true;
    this.eventsService.getAll({ page: 1, pageSize: 3 }).subscribe({
      next: (res) => {
        this.recentEvents = res.data.map((ev) => ({
          id: (ev as any).id,
          name: (ev as any).eventDesc ?? `אירוע #${(ev as any).id}`,
          date: (ev as any).eventDate,
          owner: (ev as any).officerId ? `קצין ${(ev as any).officerId}` : '—',
        }));
        this.loading = false;
      },
      error: () => {
        this.error = 'שגיאה בטעינת אירועים';
        this.loading = false;
      },
    });
  });
}

  toggleEventsMenu() {
    this.eventsOpen = !this.eventsOpen;
  }
  closeMenus() {
    this.eventsOpen = false;
  }

  @HostListener('document:keydown.escape')
  onEsc() {
    this.closeMenus();
  }

  @HostListener('document:click', ['$event'])
  onDocClick(e: MouseEvent) {
    const target = e.target as Node | null;
    if (target && !this.el.nativeElement.contains(target)) {
      this.closeMenus();
    }
  }

  goToEvents(ev?: MouseEvent) {
    ev?.stopPropagation();
    this.closeMenus();
    this.router.navigate(['/events']);
  }

  openEvent(evItem: EventItem) {
    this.closeMenus();
    // this.router.navigate(['/events', ev.id]); // אם תרצי ניווט אמיתי
    console.log('Open event:', evItem.id);
  }

  goToSystemTables() {
    this.router.navigate(['/system-tables']);
  }

  get isEventsActive(): boolean {
    const url = this.router.url.split('?')[0]; // מתעלמים מ-query string
    return url === '/events'; // רק רשימת אירועים
  }

  logout() {
    this.auth.logout().subscribe({
      next: () => {
        this.authState.clear();
        this.router.navigate(['/login']);
      },
      error: () => {
        // even if API fails, clear local state
        this.authState.clear();
        this.router.navigate(['/login']);
        alert('שגיאה בעת התנתקות');
      },
    });
  }
}
