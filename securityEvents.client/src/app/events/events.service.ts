// src/app/events/events.service.ts
import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Event, EventQuery, PagedResponse } from './event.models';

@Injectable({ providedIn: 'root' })
export class EventsService {
  private http = inject(HttpClient);
  private baseUrl = '/api/events';

  getAll(query?: EventQuery): Observable<PagedResponse<Event>> {
    let params = new HttpParams();

    if (query) {
      const set = (k: string, v: unknown) => {
        if (v !== undefined && v !== null && v !== '') {
          params = params.set(k, String(v));
        }
      };

      set('fromDate', query.fromDate);
      set('toDate', query.toDate);
      set('formatId', query.formatId);
      set('branchNum', query.branchId);
      set('zoneId', query.zoneId);
      set('officerId', query.officerId);
      set('eventType', query.eventTypeId);
      set('subEventId', query.subEventTypeId);
      set('handleType', query.handlingId);
      set('statusId', query.statusId);

      // עימוד/מיון
      set('page', query.page);
      set('pageSize', query.pageSize);
      set('sort', query.sort);
    }

    return this.http.get<PagedResponse<Event>>(this.baseUrl, { params });
  }

  update(id: number, changes: Partial<Event>): Observable<Event> {
    return this.http.put<Event>(`${this.baseUrl}/${id}`, changes);
  }


  /** אירועים אחרונים: ברירת מחדל limit=3 */
  getRecent(limit = 3): Observable<Event[]> {
    const params = new HttpParams().set('limit', String(limit));
    return this.http.get<Event[]>(`${this.baseUrl}/recent`, { params });
  }

}



