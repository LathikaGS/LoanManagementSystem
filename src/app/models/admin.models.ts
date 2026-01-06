export interface AdminUser {
  userName: string;
  id: string;
  email: string;
  requestedRole: string;
  isApproved?: boolean;
  currentRole?: string;
  annualIncome: number;
}

export interface DashboardStats {
  totalUsers: number;
  totalLoans: number;
  activeLoans: number;
  rejectedLoans: number;
  pendingLoans: number;
  totalOutstanding: number;
}

export interface LoanApplication {
  id: number;
  userId: string;
  status: string;
  amount: number;
  loanType: {
    name: string;
  };
}
