using Microsoft.AspNetCore.Identity;

namespace LoanManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsApproved { get; set; } = false;
        public string RequestedRole { get; set; }
        public int AnnualIncome { get; set; }
    }
}
