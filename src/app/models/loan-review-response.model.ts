import { LoanStatus } from './loan-status.model';

export interface LoanReviewResponse {
  loanId: number;

  customerId: string;
  customerEmail: string;

  loanAmount: number;
  tenure: number;

  status: LoanStatus;

  appliedDate: string;

  approvedROI?: number;
  reviewedBy?: string;
  reviewedOn?: string;
  reviewRemarks?: string;

  loanType: {
    loanName: string;
    roi: number;
  };
}
