export interface User {
  id: string;
  userName: string;
  email: string;
  role: 'Admin' | 'LoanOfficer' | 'Customer';
}
