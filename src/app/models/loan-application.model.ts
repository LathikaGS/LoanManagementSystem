import { LoanType } from "./loan-type.model";
import { EMI } from './emi.model';

export interface LoanApplication {
  loanId: number;
  customerId: string;
  loanTypeId: number;
  loanAmount: number;
  tenure: number;
  status: string;
  appliedDate: string;

  loanType?: LoanType;
  emis?: EMI[];
}
