import { LoanStatus } from './loan-status.model';

export interface ApproveLoanDTO {
  status: LoanStatus;
  remarks?: string;
}
