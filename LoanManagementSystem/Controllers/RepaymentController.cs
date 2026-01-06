using LoanManagementSystem.Data;
using LoanManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoanManagementSystem.Controllers
{
    [Authorize(Roles = "Customer")]
    [ApiController]
    [Route("api/repayment")]
    public class RepaymentController : ControllerBase
    {
        private readonly AppDbContext _context;
        public RepaymentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("pay/{emiId}")]
        public async Task<IActionResult> PayEmi(int emiId)
        {
            var emi = await _context.EMIs
                .Include(e => e.Loan)
                .FirstOrDefaultAsync(e => e.EMIId == emiId);

            if (emi == null)
            {
                return NotFound("EMI not found");
            }

            if (emi.PaidStatus)
            {
                return BadRequest("EMI already paid");
            }

            emi.PaidStatus = true;
            emi.PaidOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            bool allPaid = await _context.EMIs
                .Where(e => e.LoanId == emi.LoanId)
                .AllAsync(e => e.PaidStatus);

            if (allPaid)
            {
                emi.Loan.Status = LoanStatus.Closed;
                await _context.SaveChangesAsync();
            }

            var outstandingBalance = await _context.EMIs
                .Where(e => e.LoanId == emi.LoanId && !e.PaidStatus)
                .SumAsync(e => e.Amount);


            return Ok(new
            {
                Message = "EMI Payment successful",
                LoanStatus = emi.Loan.Status,
                OutstandingBalance = outstandingBalance
            });
        }
    }
}
