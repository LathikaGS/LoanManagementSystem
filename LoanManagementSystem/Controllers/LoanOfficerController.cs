using LoanManagementSystem.Data;
using LoanManagementSystem.DTOs;
using LoanManagementSystem.Models;
using LoanManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoanManagementSystem.Controllers
{
    [Authorize(Roles = "LoanOfficer")]
    [ApiController]
    [Route("api/officer")]
    public class LoanOfficerController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly EMICalculator _emi;

        public LoanOfficerController(AppDbContext context, EMICalculator emi)
        {
            _context = context;
            _emi = emi;
        }

        // ================= GET ALL APPLICATIONS =================
        [HttpGet("applications")]
        public async Task<IActionResult> GetApplications()
        {
            var loans = await _context.LoanApplications
                .Include(l => l.User)
                .Include(l => l.LoanType)
                .AsNoTracking()
                .Select(l => new LoanReviewResponseDTO
                {
                    LoanId = l.LoanId,
                    CustomerEmail = l.User!.Email,
                    LoanAmount = l.LoanAmount,
                    Tenure = l.Tenure,
                    Status = l.Status,
                    AppliedDate = l.AppliedDate,
                    ReviewedOn = l.ReviewedOn,
                    ReviewedBy = l.ReviewdBy,
                    ReviewRemarks = l.ReviewRemarks,
                    LoanType = new LoanTypeDTO 
                    {
                        LoanName = l.LoanType!.LoanName,
                        ROI = l.LoanType.ROI  
                    }
                })
                .ToListAsync();

            return Ok(loans);
        }

        // ================= GET BY STATUS =================
        [HttpGet("applications/status/{status}")]
        public async Task<IActionResult> GetByStatus(LoanStatus status)
        {
            var loans = await _context.LoanApplications
                .Where(l => l.Status == status)
                .Include(l => l.User)
                .Include(l => l.LoanType)
                .AsNoTracking()
                .Select(l => new LoanReviewResponseDTO
                {
                    LoanId = l.LoanId,
                    CustomerEmail = l.User!.Email,
                    LoanAmount = l.LoanAmount,
                    Tenure = l.Tenure,
                    Status = l.Status,
                    AppliedDate = l.AppliedDate,
                    ReviewedOn = l.ReviewedOn,
                    ReviewedBy = l.ReviewdBy,
                    ReviewRemarks = l.ReviewRemarks,
                    LoanType = new LoanTypeDTO
                    {
                        LoanName = l.LoanType!.LoanName,
                        ROI = l.LoanType.ROI  
                    }

                })
                .ToListAsync();

            return Ok(loans);
        }

        // ================= MARK UNDER REVIEW =================
        [HttpPut("under-review/{loanId}")]
        public async Task<IActionResult> MarkUnderReview(int loanId, [FromBody] ReviewRemarksDTO dto)
        {
            var loan = await _context.LoanApplications
                .FirstOrDefaultAsync(l => l.LoanId == loanId);

            if (loan == null)
                return NotFound("Loan not found");

            if (loan.Status != LoanStatus.Applied)
                return BadRequest("Loan is already under review or processed");

            loan.Status = LoanStatus.UnderReview;
            loan.ReviewedOn = DateTime.UtcNow;
            loan.ReviewdBy = User.Identity!.Name;
            loan.ReviewRemarks = dto.Remarks;

            await _context.SaveChangesAsync();

            return Ok(new LoanReviewResponseDTO
            {
                LoanId = loan.LoanId,
                LoanAmount = loan.LoanAmount,
                Tenure = loan.Tenure,
                Status = loan.Status,
                AppliedDate = loan.AppliedDate,
                ReviewedOn = loan.ReviewedOn,
                ReviewedBy = loan.ReviewdBy,
                ReviewRemarks = loan.ReviewRemarks,
                LoanType = new LoanTypeDTO
                {
                    LoanName = loan.LoanType!.LoanName,
                    ROI = loan.LoanType.ROI  
                }

            });
        }

        // ================= REVIEW / APPROVE / REJECT =================
        [HttpPut("review/{loanId}")]
        public async Task<IActionResult> ReviewLoan(int loanId, [FromBody] ApproveLoanDTO dto)
        {
            var loan = await _context.LoanApplications
                .Include(l => l.LoanType)
                .FirstOrDefaultAsync(l => l.LoanId == loanId);

            if (loan == null)
                return NotFound("Loan not found");

            if (loan.Status == LoanStatus.Approved || loan.Status == LoanStatus.Rejected)
                return BadRequest("Loan already processed");

            loan.ReviewedOn = DateTime.UtcNow;
            loan.ReviewdBy = User.Identity!.Name;
            loan.ReviewRemarks = dto.Remarks;

            if (dto.Status == LoanStatus.Rejected)
            {
                loan.Status = LoanStatus.Rejected;
            }
            else if (dto.Status == LoanStatus.Approved)
            {
                loan.Status = LoanStatus.Approved;

                var roi = loan.LoanType!.ROI;

                var emiAmount = _emi.Calculate(
                    loan.LoanAmount,
                    roi,
                    loan.Tenure
                );

                for (int i = 1; i <= loan.Tenure; i++)
                {
                    _context.EMIs.Add(new EMI
                    {
                        LoanId = loan.LoanId,
                        DueDate = DateTime.UtcNow.AddMonths(i),
                        Amount = emiAmount,
                        PaidStatus = false
                    });
                }
            }
            else
            {
                return BadRequest("Invalid status");
            }

            await _context.SaveChangesAsync();

            return Ok(new LoanReviewResponseDTO
            {
                LoanId = loan.LoanId,
                LoanAmount = loan.LoanAmount,
                Tenure = loan.Tenure,
                Status = loan.Status,
                AppliedDate = loan.AppliedDate,
                ReviewedOn = loan.ReviewedOn,
                ReviewedBy = loan.ReviewdBy,
                ReviewRemarks = loan.ReviewRemarks,
                LoanType = new LoanTypeDTO
                {
                    LoanName = loan.LoanType!.LoanName,
                    ROI = loan.LoanType.ROI
                }
            });
        }

    }
}
