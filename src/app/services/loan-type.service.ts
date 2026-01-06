import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LoanType } from '../models/loan-type.model';

@Injectable({
  providedIn: 'root'
})
export class LoanTypeService {

  private baseUrl = 'http://localhost:5228/api/loan-types';

  constructor(private http: HttpClient) {}

  getAll(): Observable<LoanType[]> {
    return this.http.get<LoanType[]>(this.baseUrl);
  }

  create(data: LoanType): Observable<any> {
    return this.http.post(this.baseUrl, data);
  }

  update(id: number, data: LoanType): Observable<any> {
    return this.http.put(`${this.baseUrl}/${id}`, data);
  }

  delete(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${id}`);
  }
}
