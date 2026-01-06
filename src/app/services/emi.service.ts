import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Loan } from '../models/loan.model';
import { EMI } from '../models/emi.model';

@Injectable({
  providedIn: 'root'
})
export class EmiService {
  private baseUrl = 'http://localhost:5228/api/loan'; 

  constructor(private http: HttpClient) {}

  getLoans(): Observable<Loan[]> {
    return this.http.get<Loan[]>(`${this.baseUrl}/loans`);
  }

  getEmis(loanId: number): Observable<EMI[]> {
    return this.http.get<EMI[]>(`${this.baseUrl}/${loanId}`);
  }

  payEmi(emiId: number): Observable<any> {
    return this.http.put(`${this.baseUrl}/pay/${emiId}`, {});
  }

  payAllEmis(loanId: number): Observable<any> {
    return this.http.post(`${this.baseUrl}/pay-all/${loanId}`, {});
  }

  getLoansByStatus(): Observable<{ status: string; count: number }[]> {
  return this.http.get<{ status: string; count: number }[]>(
    `${this.baseUrl}/status`
  );
}

}
