using LoanManagementSystem.Data;
using LoanManagementSystem.DTOs;
using LoanManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LoanManagementSystem.Controllers
{
    [Authorize(Roles = "Customer")]
    [ApiController]
    [Route("api/loan")]
    public class EMIController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EMIController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ================= MY LOANS =================
        [HttpGet("loans")]
        public async Task<IActionResult> GetMyLoans()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var loans = await _context.LoanApplications
                .Where(l => l.CustomerId == userId)
                .Include(l => l.LoanType)
                .Include(l => l.EMIs)
                .OrderByDescending(l => l.AppliedDate)
                .AsNoTracking()
                .Select(l => new CustomerLoanResponseDTO
                {
                    LoanId = l.LoanId,
                    LoanAmount = l.LoanAmount,
                    Tenure = l.Tenure,
                    Status = l.Status,

                    AppliedDate = l.AppliedDate,
                    ReviewedOn = l.ReviewedOn,

                    ReviewedBy = l.ReviewdBy,
                    ReviewRemarks = l.ReviewRemarks,

                    LoanType = l.LoanType!.LoanName,
                    BaseROI = l.LoanType.ROI,

                    TotalEMIs = l.EMIs.Count,
                    PaidEMIs = l.EMIs.Count(e => e.PaidStatus),
                    PendingEMIs = l.EMIs.Count(e => !e.PaidStatus),

                    TotalPaidAmount = l.EMIs
                        .Where(e => e.PaidStatus)
                        .Sum(e => e.Amount),

                    RemainingAmount = l.EMIs
                        .Where(e => !e.PaidStatus)
                        .Sum(e => e.Amount)
                })
                .ToListAsync();

            return Ok(loans);
        }

        // ================= LOAN STATUS COUNT =================
        [HttpGet("status")]
        public async Task<IActionResult> LoanByStatus()
        {
            var result = await _context.LoanApplications
                .GroupBy(l => l.Status)
                .Select(g => new
                {
                    Status = g.Key.ToString(),
                    Count = g.Count()
                })
                .ToListAsync();

            return Ok(result);
        }

        // ================= EMIs BY LOAN =================
        [HttpGet("{loanId}")]
        public async Task<IActionResult> GetEmisByLoan(int loanId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var emis = await _context.EMIs
                .Include(e => e.Loan)
                .Where(e => e.LoanId == loanId && e.Loan.CustomerId == userId)
                .OrderBy(e => e.DueDate)
                .AsNoTracking()
                .Select(e => new EmiResponseDTO
                {
                    EMIId = e.EMIId,
                    DueDate = e.DueDate,
                    Amount = e.Amount,
                    PaidStatus = e.PaidStatus,
                    PaidOn = e.PaidOn
                })
                .ToListAsync();

            return Ok(emis);
        }

        // ================= PAY SINGLE EMI =================
        [HttpPut("pay/{emiId}")]
        public async Task<IActionResult> PaySingleEmi(int emiId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var emi = await _context.EMIs
                .Include(e => e.Loan)
                .FirstOrDefaultAsync(e =>
                    e.EMIId == emiId &&
                    e.Loan.CustomerId == userId);

            if (emi == null)
                return NotFound("EMI not found");

            if (emi.Loan.Status != LoanStatus.Approved)
                return BadRequest("Loan not approved yet");

            if (emi.PaidStatus)
                return BadRequest("EMI already paid");

            emi.PaidStatus = true;
            emi.PaidOn = DateTime.UtcNow;

            bool allPaid = await _context.EMIs
                .Where(e => e.LoanId == emi.LoanId)
                .AllAsync(e => e.PaidStatus);

            if (allPaid)
                emi.Loan.Status = LoanStatus.Closed;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "EMI payment successful",
                emiId = emi.EMIId,
                paidOn = emi.PaidOn,
                loanStatus = emi.Loan.Status
            });
        }

        // ================= PAY ALL EMIs =================
        [HttpPost("pay-all/{loanId}")]
        public async Task<IActionResult> PayAllEmis(int loanId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var loan = await _context.LoanApplications
                .Include(l => l.EMIs)
                .FirstOrDefaultAsync(l =>
                    l.LoanId == loanId &&
                    l.CustomerId == userId);

            if (loan == null)
                return NotFound("Loan not found");

            if (loan.Status != LoanStatus.Approved)
                return BadRequest("Loan is not approved");

            var pendingEmis = loan.EMIs.Where(e => !e.PaidStatus).ToList();

            if (!pendingEmis.Any())
                return BadRequest("All EMIs already paid");

            foreach (var emi in pendingEmis)
            {
                emi.PaidStatus = true;
                emi.PaidOn = DateTime.UtcNow;
            }

            loan.Status = LoanStatus.Closed;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "All EMIs paid successfully",
                loanId,
                closed = true,
                paidCount = pendingEmis.Count
            });
        }
    }
}
