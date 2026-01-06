import { LoanReviewResponse } from '../models/loan-review-response.model';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { LoanStatus } from '../models/loan-status.model';
import { HttpClient } from '@angular/common/http';
import { ApproveLoanDTO } from '../models/approve-loan.model';
import { ReviewRemarksDTO } from '../models/review-remarks';
@Injectable({ providedIn: 'root' })
export class LoanOfficerService {
  private baseUrl = 'http://localhost:5228/api/officer';

  constructor(private http: HttpClient) {}

  getApplications(): Observable<LoanReviewResponse[]> {
  return this.http.get<LoanReviewResponse[]>(
    `${this.baseUrl}/applications`
  );
}

getByStatus(status: LoanStatus): Observable<LoanReviewResponse[]> {
  return this.http.get<LoanReviewResponse[]>(
    `${this.baseUrl}/applications/status/${status}`
  );
}


  reviewLoan(
    loanId: number,
    dto: ApproveLoanDTO
  ): Observable<any> {
    return this.http.put(
      `${this.baseUrl}/review/${loanId}`,
      dto
    );
  }

  markUnderReview(
    loanId: number,
    dto: ReviewRemarksDTO
  ): Observable<any> {
    return this.http.put(
      `${this.baseUrl}/under-review/${loanId}`,
      dto
    );
  }
}
