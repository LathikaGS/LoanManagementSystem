namespace LoanManagementSystem.DTOs
{
    public class PendingUserDTO
    {
        public string Id { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int AnnualIncome { get; set; }
    }
}
