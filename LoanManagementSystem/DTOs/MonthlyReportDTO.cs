namespace LoanManagementSystem.DTOs
{
    public class MonthlyReportDTO
    {
        public string CustomerEmail { get; set; }
        public int LoanId { get; set; }
        public string LoanType { get; set; }
        public decimal EMIAmount { get; set; }
        public bool PaidStatus { get; set; }
        public DateTime DueDate { get; set; }
    }
}
