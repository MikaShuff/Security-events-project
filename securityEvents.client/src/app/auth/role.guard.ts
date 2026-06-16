// src/app/auth/role.guard.ts
import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { AuthState } from './auth-state.service';

export function roleGuard(allowedRoles: Array<string>): CanActivateFn {
  return () => {
    const authState = inject(AuthState);
    const router = inject(Router);

    return authState.me$.pipe(
      map((me) => {
        const role = me?.role;
        const ok = !!role && allowedRoles.includes(role);
        if (!ok) router.navigate(['/events']); // send unauthorized users to events
        return ok;
      }),
      catchError(() => {
        router.navigate(['/login']);
        return of(false);
      })
    );
  };
}