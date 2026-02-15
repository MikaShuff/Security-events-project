// src/app/core/header/header.component.ts
import { Component, HostListener, ElementRef, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { EventsService } from '../events/events.service'; 
interface EventItem {
  id: number;
  name: string;
  date: string;  
  owner: string;
}

@Component({
  standalone: true,
  selector: 'app-header',
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {
  eventsOpen = false;
  recentEvents: EventItem[] = [];
  loading = true;
  error = '';

  constructor(
    private el: ElementRef,
    private eventsService: EventsService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.eventsService.getAll({ page: 1, pageSize: 3 }).subscribe({
      next: (res) => {

        this.recentEvents = res.data.map(ev => ({
          id: (ev as any).id,
          name: (ev as any).eventDesc ?? `אירוע #${(ev as any).id}`,
          date: (ev as any).eventDate, 
          owner: (ev as any).officerId ? `קצין ${(ev as any).officerId}` : '—'
        }));
        this.loading = false;
      },
      error: () => {
        this.error = 'שגיאה בטעינת אירועים';
        this.loading = false;
      }
    });
  }

  toggleEventsMenu() { this.eventsOpen = !this.eventsOpen; }
  closeMenus() { this.eventsOpen = false; }

  @HostListener('document:keydown.escape')
  onEsc() { this.closeMenus(); }

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

  openNewEvent(ev?: MouseEvent) {
    ev?.stopPropagation();
    this.closeMenus();
    this.router.navigate(['/tickets/open']);
  }


  openEvent(evItem: EventItem) {
    this.closeMenus();
    // this.router.navigate(['/events', ev.id]); // אם תרצי ניווט אמיתי
    console.log('Open event:', evItem.id);
  }


  goToSystemTables() {
    this.router.navigate(['/system-tables']);
  }

}
