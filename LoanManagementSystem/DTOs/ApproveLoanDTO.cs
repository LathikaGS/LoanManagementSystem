using LoanManagementSystem.Models;

namespace LoanManagementSystem.DTOs
{
    public class ApproveLoanDTO
    {
        public LoanStatus Status { get; set; }
        public string? Remarks { get; set; }
    }
}
