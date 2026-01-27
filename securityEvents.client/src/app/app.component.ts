
// src/app/app.component.ts
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HeaderComponent } from './header/header.component'; // אם ההדר שלך נמצא ב-app/header

@Component({
  selector: 'app-root',
  standalone: true,
  templateUrl: './app.component.html',
  imports: [RouterOutlet, HeaderComponent, CommonModule], // רק פעם אחת
})
export class AppComponent {}
