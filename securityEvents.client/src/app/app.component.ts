// src/app/app.component.ts
import { Component, AfterViewChecked, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, NavigationEnd, RouterOutlet } from '@angular/router';
import { filter, map, startWith } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { HeaderComponent } from './header/header.component';

@Component({
  selector: 'app-root',
  standalone: true,
  templateUrl: './app.component.html',
  imports: [RouterOutlet, HeaderComponent, CommonModule],
})
export class AppComponent implements AfterViewChecked {
  showHeader$: Observable<boolean>;
  private lastHeaderHeight = -1;

  constructor(private router: Router) {
    this.showHeader$ = this.router.events.pipe(
      filter((e): e is NavigationEnd => e instanceof NavigationEnd),
      startWith(null),
      map(() => !this.router.url.startsWith('/login'))
    );
  }

  ngAfterViewChecked(): void {
    this.updateHeaderHeight();
  }

  @HostListener('window:resize')
  onResize() {
    this.updateHeaderHeight();
  }

  private updateHeaderHeight() {
    const el = document.getElementById('app-header');
    const h = el?.offsetHeight ?? 0;

    // prevent constantly writing the CSS var
    if (h !== this.lastHeaderHeight) {
      this.lastHeaderHeight = h;
      document.documentElement.style.setProperty('--header-height', `${h}px`);
    }
  }
}