export interface LoanType{
    loanTypeId? : number;
    loanName: string;
    roi: number;
    maxTenure: number;
    minAmount: number;
    maxAmount: number;
}