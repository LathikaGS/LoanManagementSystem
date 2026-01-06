import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AdminUser } from '../models/admin.models';
import { DashboardStats } from '../models/admin.models';
import { LoanApplication } from '../models/loan-application.model';

@Injectable({
  providedIn: 'root',
})
export class Admin {
  private baseUrl = 'http://localhost:5228/api/admin';

  constructor(private http: HttpClient) {}

  getAllUsers(): Observable<AdminUser[]> {
    return this.http.get<AdminUser[]>(`${this.baseUrl}/users`);
  }

  getPendingUsers(): Observable<AdminUser[]> {
  return this.http.get<AdminUser[]>(`${this.baseUrl}/pending-users`);
}

  approveUser(userId: string): Observable<string> {
    return this.http.post(`${this.baseUrl}/approve-user/${userId}`, {}, { responseType: 'text' });
  }

  rejectUser(userId: string): Observable<string> {
    return this.http.delete(`${this.baseUrl}/reject-user/${userId}`, { responseType: 'text' });
  }

  disableUser(id: string) {
  return this.http.put(`${this.baseUrl}/disable-user/${id}`, {});
}

enableUser(id: string) {
  return this.http.put(`${this.baseUrl}/enable-user/${id}`, {});
}

  getDashboard(): Observable<DashboardStats> {
    return this.http.get<DashboardStats>(`${this.baseUrl}/dashboard`);
  }

  getApplications(): Observable<LoanApplication[]> {
    return this.http.get<LoanApplication[]>(`${this.baseUrl}/applications`);
  }

  getLoanEmis(loanId: number) {
    return this.http.get(`${this.baseUrl}/loan/${loanId}/emis`);
  }
}

