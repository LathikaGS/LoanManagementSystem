import { EMI } from "./emi.model";
import { LoanStatus } from "./loan-status.model";
export interface Loan {
  loanId: number;
  loanAmount: number;
  tenure: number;
  status: LoanStatus;
  appliedDate: string;

  loanType: string;     
  baseROI: number;      

  totalPaidAmount: number;
  remainingAmount: number;
  EMIs?: EMI[];
}
