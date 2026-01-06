using System.ComponentModel.DataAnnotations;

public class LoanTypeDTO
{
    [Required]
    public string LoanName { get; set; }

    [Range(0.01, 100, ErrorMessage = "Interest rate must be positive")]
    public decimal ROI { get; set; }

    [Range(1, 60, ErrorMessage = "Max tenure must be at least 1 month")]
    public int MaxTenure { get; set; }
    public decimal MinAmount { get; set;  }
    public decimal MaxAmount { get; set; }
}
