import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  ReportsSummaryData,
  OutstandingResponse,
  LoanStatusReport,
  OverdueEmi
} from './../models/reports.model';

@Injectable({ providedIn: 'root' })
export class ReportsService {

  private readonly baseUrl = 'http://localhost:5228/api/reports';

  constructor(private http: HttpClient) {}

  getSummary(): Observable<ReportsSummaryData> {
    return this.http.get<ReportsSummaryData>(`${this.baseUrl}/summary`);
  }

  getOutstanding(): Observable<OutstandingResponse> {
    return this.http.get<OutstandingResponse>(`${this.baseUrl}/outstanding`);
  }

  getLoanStatus(): Observable<LoanStatusReport[]> {
    return this.http.get<LoanStatusReport[]>(`${this.baseUrl}/status`);
  }

  getOverdueEmis(): Observable<OverdueEmi[]> {
    return this.http.get<OverdueEmi[]>(`${this.baseUrl}/overdue`);
  }

  getMonthly(month: number, year: number): Observable<any> {
    return this.http.get<any>(
      `${this.baseUrl}/monthly`,
      { params: { month, year } }
    );
  }

  getMonthlyGrouped(month: number, year: number): Observable<any[]> {
    return this.http.get<any[]>(
      `${this.baseUrl}/monthly/grouped`,
      { params: { month, year } }
    );
  }
}
