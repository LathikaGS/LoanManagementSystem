using System.Globalization;

namespace LoanManagementSystem.DTOs
{
    public class UserDTO
    {
        public string UserName { get; set; }
        public string Id { get; set; }
        public string Email { get; set; }
        public bool IsApproved { get; set; }
        public string RequestedRole { get; set; }
        public string CurrentRole { get; set; }
        public int AnnualIncome { get; set; }
    }
}
