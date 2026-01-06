using LoanManagementSystem.Models;

namespace LoanManagementSystem.DTOs
{
    public class LoanReviewResponseDTO
    {
        public int LoanId { get; set; }

        public string CustomerEmail { get; set; } = string.Empty;

        public decimal LoanAmount { get; set; }
        public int Tenure { get; set; }
        public LoanStatus Status { get; set; }
        public DateTime AppliedDate { get; set; }

        public DateTime? ReviewedOn { get; set; }
        public string? ReviewedBy { get; set; }
        public string? ReviewRemarks { get; set; }

        public LoanTypeDTO LoanType { get; set; } 
    }
}
