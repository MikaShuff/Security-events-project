// auth-state.service.ts
import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError,of, tap } from 'rxjs';
import { AuthService, MeResponse } from './auth.service';

@Injectable({ providedIn: 'root' })
export class AuthState {
  private meSubject = new BehaviorSubject<MeResponse | null>(null);
  me$ = this.meSubject.asObservable();

  constructor(private auth: AuthService) {}

  refreshMe() {
    return this.auth.me().pipe(
      tap((me) => this.meSubject.next(me)),
      catchError((err) => {
        if (err.status === 401) {
          // normal: not logged in
          this.meSubject.next(null);
          return of(null);
        }
        // unexpected error -> still clear, but let it bubble if you want
        this.meSubject.next(null);
        return of(null); // or: throwError(() => err)
      }),
    );
  }

  clear() {
    this.meSubject.next(null);
  }
}
