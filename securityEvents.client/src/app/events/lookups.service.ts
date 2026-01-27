
// src/app/events/lookups.service.ts

import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, shareReplay } from 'rxjs';
import { LookupItem } from './event.models';

@Injectable({ providedIn: 'root' })
export class LookupsService {
  private http = inject(HttpClient);

  private cache = new Map<string, Observable<LookupItem[]>>();

  private fetchOnce(path: string): Observable<LookupItem[]> {
    if (!this.cache.has(path)) {
      this.cache.set(path, this.http.get<LookupItem[]>(path).pipe(shareReplay(1)));
    }
    return this.cache.get(path)!;
  }

  getFormats() { return this.fetchOnce('/api/formats'); }
  getBranches() { return this.fetchOnce('/api/branches'); }
  getZones() { return this.fetchOnce('/api/zones'); }
  getOfficers() { return this.fetchOnce('/api/officers'); }
  getEventTypes() { return this.fetchOnce('/api/event-types'); }
  getSubEventTypes() { return this.fetchOnce('/api/sub-event-types'); }
  getHandlings() { return this.fetchOnce('/api/handlings'); }
  getStatuses() { return this.fetchOnce('/api/statuses'); }
}
