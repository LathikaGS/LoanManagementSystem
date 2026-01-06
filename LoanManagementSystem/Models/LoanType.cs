namespace LoanManagementSystem.Models
{
    public class LoanType
    {
        public int LoanTypeId { get; set; }

        public string LoanName { get; set; }

        public decimal ROI { get; set; }

        public int MaxTenure { get; set; }

        public decimal MinAmount { get; set; }

        public decimal MaxAmount { get; set; }

        public ICollection<LoanApplication>? LoanApplications { get; set; }
    }
}
