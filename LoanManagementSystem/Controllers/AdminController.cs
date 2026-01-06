using LoanManagementSystem.Data;
using LoanManagementSystem.DTOs;
using LoanManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoanManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(
            AppDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ---------------- ALL USERS ----------------
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var result = new List<UserDTO>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                result.Add(new UserDTO
                {
                    Id = user.Id,
                    UserName = user.UserName ?? "",
                    Email = user.Email ?? "",
                    IsApproved = user.IsApproved,
                    RequestedRole = user.RequestedRole,
                    CurrentRole = roles.FirstOrDefault(),
                    AnnualIncome = user.AnnualIncome
                });
            }

            return Ok(result);
        }

        // ---------------- PENDING USERS ----------------
        [HttpGet("pending-users")]
        public async Task<IActionResult> GetPendingUsers()
        {
            var users = await _userManager.Users
                .Where(u => !u.IsApproved)
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.RequestedRole,
                    u.AnnualIncome
                })
                .ToListAsync();

            return Ok(users);
        }

        // ---------------- APPROVE USER ----------------
        [HttpPost("approve-user/{userId}")]
        public async Task<IActionResult> ApproveUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound("User not found");

            user.IsApproved = true;

            await _userManager.AddToRoleAsync(user, user.RequestedRole);
            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                message = $"User approved as {user.RequestedRole}"
            });
        }

        // ---------------- REJECT USER ----------------
        [HttpDelete("reject-user/{userId}")]
        public async Task<IActionResult> RejectUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound("User not found");

            await _userManager.DeleteAsync(user);

            return Ok(new { message = "User rejected and removed" });
        }

        // ---------------- DISABLE USER ----------------
        [HttpPut("disable-user/{userId}")]
        public async Task<IActionResult> DisableUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound("User not found");

            user.IsApproved = false;
            user.LockoutEnabled = true;

            await _userManager.UpdateAsync(user);
            await _userManager.SetLockoutEndDateAsync(
                user,
                DateTimeOffset.UtcNow.AddYears(100)
            );

            return Ok(new { message = "User disabled successfully" });
        }

        // ---------------- ENABLE USER ----------------
        [HttpPut("enable-user/{userId}")]
        public async Task<IActionResult> EnableUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound("User not found");

            user.IsApproved = true;

            await _userManager.UpdateAsync(user);
            await _userManager.SetLockoutEndDateAsync(user, null);

            return Ok(new { message = "User enabled successfully" });
        }

        // ---------------- APPLICATIONS ----------------
        [HttpGet("applications")]
        public async Task<IActionResult> GetAllApplications()
        {
            var data = await _context.LoanApplications
                .Include(l => l.User)
                .Include(l => l.LoanType)
                .ToListAsync();

            return Ok(data);
        }

        // ---------------- LOAN EMIs ----------------
        [HttpGet("loan/{loanId}/emis")]
        public async Task<IActionResult> GetLoanEmis(int loanId)
        {
            var emis = await _context.EMIs
                .Where(e => e.LoanId == loanId)
                .ToListAsync();

            return Ok(emis);
        }

        // ---------------- DASHBOARD ----------------
        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            return Ok(new
            {
                TotalUsers = await _userManager.Users.CountAsync(),
                TotalLoans = await _context.LoanApplications.CountAsync(),
                ActiveLoans = await _context.LoanApplications.CountAsync(l => l.Status == LoanStatus.Approved),
                RejectedLoans = await _context.LoanApplications.CountAsync(l => l.Status == LoanStatus.Rejected),
                PendingLoans = await _context.LoanApplications.CountAsync(l => l.Status == LoanStatus.Applied),
                TotalOutstanding = await _context.EMIs
                    .Where(e => !e.PaidStatus)
                    .SumAsync(e => e.Amount)
            });
        }
    }
}
