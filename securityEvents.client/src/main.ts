// src/main.ts
import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter, Routes } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { AppComponent } from './app/app.component';
import { EventsListComponent } from './app/events/events-list/events-list.component';
import { OpenTicketComponent } from './app/tickets/open-ticket/open-ticket.component';
import { LOCALE_ID } from '@angular/core';
import { registerLocaleData } from '@angular/common';
import localeHe from '@angular/common/locales/he';

registerLocaleData(localeHe);

const routes: Routes = [
  { path: '', redirectTo: 'events', pathMatch: 'full' },
  { path: 'events', component: EventsListComponent },
  { path: 'tickets/open', component: OpenTicketComponent },
  { path: '**', redirectTo: 'events' },
];

bootstrapApplication(AppComponent, {
  providers: [provideRouter(routes),
    provideHttpClient(),
    provideRouter(routes),
    { provide: LOCALE_ID, useValue: 'he-IL' }, 
    //add more providers here
  ],
}).catch(err => console.error(err));


