//app/app.routes.ts

import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { OpenTicketComponent } from './tickets/open-ticket/open-ticket.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },               // דף הבית
  { path: 'tickets/open', component: OpenTicketComponent }, // פתיחת אירוע חדש
  { path: '**', redirectTo: '' }
];


