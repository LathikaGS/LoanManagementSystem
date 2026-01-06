import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoanService {

  private baseUrl = 'http://localhost:5228/api/loans';

  constructor(private http: HttpClient) {}

  getLoanTypes(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/loan-types`);
  }

  applyLoan(data: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/apply`, data);
  }

  getMyLoans(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/my-loans`);
  }
}
