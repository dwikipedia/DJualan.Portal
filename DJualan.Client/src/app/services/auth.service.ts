// src/app/services/auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import {environment} from '../../environments/environment';

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  expiresIn: number;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private tokenKey = 'jwt_token';
  private userKey = 'user_info';

  constructor(private http: HttpClient) { }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${environment.apiUrl}/auth/login`, credentials)
      .pipe(
        tap(response => {
          this.setToken(response.token);
          this.setUserInfo(response);
        })
      );
  }

  setToken(token: string): void {
    localStorage.setItem(this.tokenKey, token);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  setUserInfo(userInfo: any): void {
    localStorage.setItem(this.userKey, JSON.stringify(userInfo));
  }

  getUserInfo(): any {
    const userInfo = localStorage.getItem(this.userKey);
    return userInfo ? JSON.parse(userInfo) : null;
  }

  getCurrentUser(): string {
    const userInfo = this.getUserInfo();
    if (userInfo) {
      // Return first name if available, otherwise username
      return userInfo.firstName || userInfo.username || 'User';
    }
    return 'User';
  }

  getFullName(): string {
    const userInfo = this.getUserInfo();
    if (userInfo && userInfo.firstName && userInfo.lastName) {
      return `${userInfo.firstName} ${userInfo.lastName}`;
    } else if (userInfo && userInfo.firstName) {
      return userInfo.firstName;
    } else if (userInfo && userInfo.username) {
      return userInfo.username;
    }
    return 'User';
  }

   getUserRoles(): string[] {
    const userInfo = this.getUserInfo();
    return userInfo?.roles || [];
  }

  hasRole(role: string): boolean {
    const roles = this.getUserRoles();
    return roles.includes(role);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userKey);
  }
}