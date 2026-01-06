export interface ReportsSummaryData {
  totalLoans: number;
  approved: number;
  rejected: number;
  underReview: number;
  closed: number;
  outstandingAmount: number;
  totalCustomers: number;
}

export interface OutstandingResponse {
  totalOutstanding: number;
  currency: string;
}

export interface LoanStatusReport {
  status: string;
  count: number;
  totalAmount: number;
}

export interface OverdueEmi {
  emiId: number;
  loanId: number;
  customerEmail: string;
  loanType: string;
  amount: number;
  dueDate: string;
  daysOverdue: number;
  loanStatus: string;
}
