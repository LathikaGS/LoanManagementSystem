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
    [Route("api/loans")]
    public class LoanApplicationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public LoanApplicationController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpPost("apply")]
        public async Task<IActionResult> ApplyLoan([FromBody] ApplyLoanDTO applyLoanDTO)
        {
            if (applyLoanDTO.Amount <= 0)
                return BadRequest(new { rule = "INVALID_AMOUNT", message = "Loan amount must be greater than zero." });

            var loanType = await _context.LoanTypes.FindAsync(applyLoanDTO.LoanTypeId);
            if (loanType == null)
                return BadRequest(new { rule = "INVALID_LOAN_TYPE", message = "Selected loan type does not exist." });

            if (applyLoanDTO.Tenure <= 0)
                return BadRequest(new { rule = "INVALID_TENURE", message = "Tenure must be greater than zero." });

            if (applyLoanDTO.Amount < loanType.MinAmount || applyLoanDTO.Amount > loanType.MaxAmount)
            {
                return BadRequest(new
                {
                    rule = "AMOUNT_OUT_OF_RANGE",
                    message = $"Loan amount must be between {loanType.MinAmount} and {loanType.MaxAmount}."
                });
            }


            if (applyLoanDTO.Tenure > loanType.MaxTenure)
                return BadRequest(new
                {
                    rule = "TENURE_EXCEEDED",
                    message = $"Maximum allowed tenure is {loanType.MaxTenure} months."
                });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            bool exists = await _context.LoanApplications.AnyAsync(l =>
                l.CustomerId == userId &&
                l.LoanTypeId == applyLoanDTO.LoanTypeId &&
                l.Status == LoanStatus.Applied
            );

            if (exists)
                return BadRequest(new
                {
                    rule = "DUPLICATE_APPLICATION",
                    message = "You already have a pending application for this loan type."
                });

            var loan = new LoanApplication
            {
                CustomerId = userId,
                LoanTypeId = applyLoanDTO.LoanTypeId,
                LoanAmount = applyLoanDTO.Amount,
                Tenure = applyLoanDTO.Tenure,
                Status = LoanStatus.Applied,
                AppliedDate = DateTime.UtcNow
            };

            _context.LoanApplications.Add(loan);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Loan applied successfully",
                loanId = loan.LoanId
            });
        }


        [HttpGet("my-loans")]
        public async Task<IActionResult> GetMyLoans()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var loans = await _context.LoanApplications
                .Include(l => l.LoanType)
                .Where(l => l.CustomerId == userId)
                .Select(l => new
                {
                    l.LoanId,
                    l.LoanAmount,
                    l.Tenure,
                    l.Status,
                    l.AppliedDate,
                    LoanType = l.LoanType!.LoanName
                })
                .ToListAsync();

            return Ok(loans);
        }

        [HttpGet("loan-types")]
        public async Task<IActionResult> GetLoanTypes()
        {
            return Ok(await _context.LoanTypes.ToListAsync());
        }

        [HttpGet("{loanId}")]
        public async Task<IActionResult> GetLoanDetails(int loanId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var loan = await _context.LoanApplications
                .Include(l => l.LoanType)
                .Include(l => l.EMIs)
                .FirstOrDefaultAsync(l => l.LoanId == loanId && l.CustomerId == userId);

            if (loan == null)
                return NotFound("Loan not found");

            return Ok(loan);
        }

        

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            return Ok(new
            {
                id = user.Id,
                name = user.UserName,
                email = user.Email,
                phone = user.PhoneNumber,
                role = (await _userManager.GetRolesAsync(user)).FirstOrDefault()
            });
        }


    }
}
