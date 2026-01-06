export interface EMI {
  emiId: number;
  dueDate: string;
  amount: number;
  paidStatus: boolean;
  paidOn?: string | null;
}
