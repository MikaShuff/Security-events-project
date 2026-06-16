// auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { API_BASE } from '../config';

export type MeResponse = { username: string | null; role: string | null };

@Injectable({ providedIn: 'root' })
export class AuthService {
  constructor(private http: HttpClient) {}

  login(username: string, password: string) {
    return this.http.post<{ username: string; role: string }>(
      `${API_BASE}/api/auth/login`,
      { username, password },
      { withCredentials: true },
    );
  }

  me() {
    return this.http.get<MeResponse>(`${API_BASE}/api/auth/me`, {
      withCredentials: true,
    });
  }

  logout() {
    return this.http.post(`${API_BASE}/api/auth/logout`, {}, { withCredentials: true });
  }
}