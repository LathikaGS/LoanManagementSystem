using System.ComponentModel.DataAnnotations;

namespace LoanManagementSystem.Models
{
    public class LoanApplication
    {
        [Key]
        public int LoanId { get; set; }
        public string CustomerId { get; set; }
        public ApplicationUser User { get; set; }
        public int LoanTypeId { get; set; }
        public LoanType LoanType { get; set; }
        public decimal LoanAmount { get; set; }
        public int Tenure {  get; set; }
        public LoanStatus Status { get; set; }
        public DateTime AppliedDate { get; set; } = DateTime.Now;
        public ICollection<EMI> EMIs { get; set; }
        public decimal MonthlyIncome { get; set; }
        public string? ReviewRemarks { get; set; }
        public DateTime? ReviewedOn { get; set; }
        public string? ReviewdBy { get; set; }
        
    }
}
