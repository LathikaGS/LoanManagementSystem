using LoanManagementSystem.Data;
using LoanManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoanManagementSystem.Controllers
{
    [Authorize(Roles = "LoanOfficer")]
    [ApiController]
    [Route("api/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }

        // ================= TOTAL OUTSTANDING =================
        [HttpGet("outstanding")]
        public async Task<IActionResult> OutstandingAmount()
        {
            var totalOutstanding = await _context.EMIs
                .Where(e => !e.PaidStatus)
                .Select(e => (decimal?)e.Amount)
                .SumAsync() ?? 0;

            return Ok(new
            {
                totalOutstanding,
                currency = "INR"
            });
        }

        // ================= LOANS COUNT BY STATUS =================
        [HttpGet("status")]
        public async Task<IActionResult> LoanByStatus()
        {
            var data = await _context.LoanApplications
                .GroupBy(l => l.Status)
                .Select(g => new
                {
                    status = g.Key.ToString(),
                    count = g.Count(),
                    totalAmount = g.Sum(x => x.LoanAmount)
                })
                .OrderByDescending(x => x.count)
                .ToListAsync();

            return Ok(data);
        }

        // ================= MONTHLY EMI REPORT =================
        [HttpGet("monthly")]
        public async Task<IActionResult> MonthlyReport(int month, int year)
        {
            if (month < 1 || month > 12)
                return BadRequest("Invalid month");

            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1);

            var records = await _context.EMIs
                .Include(e => e.Loan).ThenInclude(l => l.User)
                .Include(e => e.Loan).ThenInclude(l => l.LoanType)
                .Where(e => e.DueDate >= start && e.DueDate < end)
                .Select(e => new
                {
                    e.EMIId,
                    e.LoanId,
                    customerEmail = e.Loan.User.Email,
                    loanType = e.Loan.LoanType.LoanName,
                    emiAmount = e.Amount,
                    paidStatus = e.PaidStatus,
                    dueDate = e.DueDate
                })
                .OrderBy(e => e.dueDate)
                .ToListAsync();

            return Ok(new
            {
                month,
                year,
                totalEmis = records.Count,
                paidEmis = records.Count(x => x.paidStatus),
                pendingEmis = records.Count(x => !x.paidStatus),
                totalAmount = records.Sum(x => x.emiAmount),
                records
            });
        }

        // ================= MONTHLY GROUPED BY CUSTOMER =================
        [HttpGet("monthly/grouped")]
        public async Task<IActionResult> MonthlyGrouped(int month, int year)
        {
            if (month < 1 || month > 12)
                return BadRequest("Invalid month");

            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1);

            var data = await _context.EMIs
                .Include(e => e.Loan).ThenInclude(l => l.User)
                .Where(e => e.DueDate >= start && e.DueDate < end)
                .GroupBy(e => e.Loan.User.Email)
                .Select(g => new
                {
                    customerEmail = g.Key,
                    totalEmis = g.Count(),
                    paidAmount = g.Where(x => x.PaidStatus).Sum(x => x.Amount),
                    pendingAmount = g.Where(x => !x.PaidStatus).Sum(x => x.Amount)
                })
                .OrderByDescending(x => x.pendingAmount)
                .ToListAsync();

            return Ok(data);
        }

        // ================= OVERDUE EMIs =================
        [HttpGet("overdue")]
        public async Task<IActionResult> OverdueEmis()
        {
            var today = DateTime.UtcNow;

            var overdue = await _context.EMIs
                .Include(e => e.Loan).ThenInclude(l => l.User)
                .Include(e => e.Loan).ThenInclude(l => l.LoanType)
                .Where(e => !e.PaidStatus && e.DueDate < today)
                .Select(e => new
                {
                    e.EMIId,
                    e.LoanId,
                    customerEmail = e.Loan.User.Email,
                    loanType = e.Loan.LoanType.LoanName,
                    amount = e.Amount,
                    dueDate = e.DueDate,
                    daysOverdue = EF.Functions.DateDiffDay(e.DueDate, today),
                    loanStatus = e.Loan.Status.ToString()
                })
                .OrderByDescending(e => e.daysOverdue)
                .ToListAsync();

            return Ok(overdue);
        }

        // ================= CUSTOMER REPORT =================
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> CustomerReport(string customerId)
        {
            var loans = await _context.LoanApplications
                .Include(l => l.LoanType)
                .Include(l => l.EMIs)
                .Where(l => l.CustomerId == customerId)
                .Select(l => new
                {
                    l.LoanId,
                    loanType = l.LoanType.LoanName,
                    l.LoanAmount,
                    l.Tenure,
                    status = l.Status.ToString(),
                    l.AppliedDate,
                    l.ReviewedOn,
                    l.ReviewRemarks,
                    totalEmis = l.EMIs.Count,
                    paidEmis = l.EMIs.Count(e => e.PaidStatus),
                    pendingEmis = l.EMIs.Count(e => !e.PaidStatus),
                    paidAmount = l.EMIs.Where(e => e.PaidStatus).Sum(e => e.Amount),
                    pendingAmount = l.EMIs.Where(e => !e.PaidStatus).Sum(e => e.Amount)
                })
                .ToListAsync();

            return Ok(loans);
        }

        // ================= SUMMARY REPORT =================
        [HttpGet("summary")]
        public async Task<IActionResult> SummaryReport()
        {
            // 1️⃣ Get Customer Role Id
            var customerRoleId = await _context.Roles
                .Where(r => r.Name == "Customer")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            var totalCustomers = customerRoleId == null
                ? 0
                : await _context.UserRoles.CountAsync(ur => ur.RoleId == customerRoleId);

            var summary = new
            {
                totalLoans = await _context.LoanApplications.CountAsync(),
                approved = await _context.LoanApplications.CountAsync(l => l.Status == LoanStatus.Approved),
                rejected = await _context.LoanApplications.CountAsync(l => l.Status == LoanStatus.Rejected),
                underReview = await _context.LoanApplications.CountAsync(l => l.Status == LoanStatus.UnderReview),
                closed = await _context.LoanApplications.CountAsync(l => l.Status == LoanStatus.Closed),
                outstandingAmount = await _context.EMIs
                    .Where(e => !e.PaidStatus)
                    .Select(e => (decimal?)e.Amount)
                    .SumAsync() ?? 0,
                totalCustomers
            };

            return Ok(summary);
        }
    }
}

