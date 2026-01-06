using System.ComponentModel.DataAnnotations;

namespace LoanManagementSystem.Models
{
    public class EMI
    {
        [Key]
        public int EMIId{get; set; }
        public int LoanId { get; set; }
        public LoanApplication Loan {  get; set; }
        public DateTime DueDate { get; set; }
        public decimal Amount   { get; set; }
        public bool PaidStatus {  get; set; } = false;
        public DateTime? PaidOn { get; set; }
    }
}
