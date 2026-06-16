// src/app/layout/shell/shell.component.ts
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { AuthState } from '../../auth/auth-state.service';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './shell.component.html',
  styleUrls: ['./shell.component.css'],
})

export class ShellComponent {
  isLoggedIn$!: Observable<boolean>;

  role$!: Observable<string | null>;
  constructor(private authState: AuthState) {
    
    this.isLoggedIn$ = this.authState.me$.pipe(map(me => !!me?.username));
    this.role$ = this.authState.me$.pipe(map(me => me?.role || null));
  }
}