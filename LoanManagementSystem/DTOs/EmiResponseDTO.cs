namespace LoanManagementSystem.DTOs
{
    public class EmiResponseDTO
    {
        public int EMIId { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
        public bool PaidStatus { get; set; }
        public DateTime? PaidOn { get; set; }
    }
}
