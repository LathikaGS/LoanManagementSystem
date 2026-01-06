namespace LoanManagementSystem.DTOs
{
    public class ApplyLoanDTO
    {
        public int LoanTypeId { get; set; }
        public decimal Amount { get; set; }
        public int Tenure {  get; set; }
    }
}
