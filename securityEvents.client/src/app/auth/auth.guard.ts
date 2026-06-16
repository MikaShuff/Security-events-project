// src/app/auth/auth.guard.ts
import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { map, catchError, of } from 'rxjs';
import { AuthState } from './auth-state.service';

export const authGuard: CanActivateFn = () => {
  const authState = inject(AuthState);
  const router = inject(Router);

  return authState.refreshMe().pipe(
    map(me => (me?.username ? true : router.parseUrl('/login'))),
    catchError(() => {
      authState.clear();
      return of(router.parseUrl('/login'));
    })
  );
};