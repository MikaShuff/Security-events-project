// app/app.routes.ts
import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { OpenEventComponent } from './events/open-event/open-event.component';
import { SystemTablesComponent } from './system-tables/system-tables.component';
import { EventsListComponent } from './events/events-list/events-list.component';
import { LoginComponent } from './login/login.component';
import { authGuard } from './auth/auth.guard';
import { ShellComponent } from './layout/shell/shell.component';
import { roleGuard } from './auth/role.guard';

export const routes: Routes = [
  // public
  { path: 'login', component: LoginComponent },

  // everything else is inside the authenticated shell
  {
    path: '',
    component: ShellComponent,
    canActivate: [authGuard],
    children: [
      { path: '', component: HomeComponent },

      // everyone logged in can view events list
      { path: 'events', component: EventsListComponent },

      // only Admin + User can open new event
      { path: 'events/open', component: OpenEventComponent, canActivate: [roleGuard(['Admin', 'User'])] },

      // admin only
      {
        path: 'system-tables',
        component: SystemTablesComponent,
        canActivate: [roleGuard(['Admin'])],
        children: [
          // /system-tables -> /system-tables/event-types
          { path: '', pathMatch: 'full', redirectTo: 'event-types' },

          { path: 'branches', loadComponent: () => import('./system-tables/branches/branches.component').then(m => m.BranchesComponent) },
          { path: 'events', loadComponent: () => import('./system-tables/events/events.component').then(m => m.EventsComponent) },
          { path: 'sub-event-types', loadComponent: () => import('./system-tables/sub-event-types/sub-event-types.component').then(m => m.SubEventTypesComponent) },
          { path: 'event-types', loadComponent: () => import('./system-tables/event-types/event-types.component').then(m => m.EventTypesComponent) },
          { path: 'formats', loadComponent: () => import('./system-tables/formats/formats.component').then(m => m.FormatsComponent) },
          { path: 'security-zones', loadComponent: () => import('./system-tables/security-zones/security-zones.component').then(m => m.SecurityZonesComponent) },
          { path: 'officers', loadComponent: () => import('./system-tables/officers/officers.component').then(m => m.OfficersComponent) },
          { path: 'zones', loadComponent: () => import('./system-tables/zones/zones.component').then(m => m.ZonesComponent) },
          { path: 'handlings', loadComponent: () => import('./system-tables/handlings/handlings.component').then(m => m.HandlingsComponent) },
          { path: 'statuses', loadComponent: () => import('./system-tables/statuses/statuses.component').then(m => m.StatusesComponent) },
        ]
      },
    ],
  },
];