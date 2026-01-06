using LoanManagementSystem.Models;

namespace LoanManagementSystem.DTOs
{
    public class CustomerLoanResponseDTO
    {
        public int LoanId { get; set; }
        public decimal LoanAmount { get; set; }
        public int Tenure { get; set; }
        public LoanStatus Status { get; set; }
        public DateTime AppliedDate { get; set; }
        public DateTime? ReviewedOn { get; set; }

        public string? ReviewedBy { get; set; }
        public string? ReviewRemarks { get; set; }
        public decimal? ApprovedROI { get; set; }
        public string LoanType { get; set; } = string.Empty;
        public decimal BaseROI { get; set; }
        public int TotalEMIs { get; set; }
        public int PaidEMIs { get; set; }
        public int PendingEMIs { get; set; }

        public decimal TotalPaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
    }
}
