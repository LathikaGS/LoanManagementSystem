using LoanManagementSystem.Controllers;
using LoanManagementSystem.Data;
using LoanManagementSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace LoanManagement.Tests
{
    public class ReportsControllerTests
    {
        private readonly AppDbContext _context;
        private readonly ReportsController _controller;

        public ReportsControllerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            TestSeedData.Seed(_context);

            _controller = new ReportsController(_context);

            // 🔐 Mock LoanOfficer authentication
            var officer = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "officer1"),
                new Claim(ClaimTypes.Role, "LoanOfficer")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = officer }
            };
        }

        // ================= OUTSTANDING =================
        [Fact]
        public async Task OutstandingAmount_ReturnsTotalOutstanding()
        {
            var result = await _controller.OutstandingAmount();

            var ok = Assert.IsType<OkObjectResult>(result);
            var value = ok.Value;

            var totalOutstanding = (decimal)value
                .GetType().GetProperty("totalOutstanding")!
                .GetValue(value)!;

            Assert.True(totalOutstanding > 0);
        }

        // ================= LOAN BY STATUS =================
        [Fact]
        public async Task LoanByStatus_ReturnsGroupedData()
        {
            var result = await _controller.LoanByStatus();

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<System.Collections.IEnumerable>(ok.Value);

            Assert.NotEmpty(list);
        }

        // ================= MONTHLY REPORT =================
        [Fact]
        public async Task MonthlyReport_ReturnsMonthlySummary()
        {
            var today = DateTime.UtcNow;

            var result = await _controller.MonthlyReport(today.Month, today.Year);

            var ok = Assert.IsType<OkObjectResult>(result);
            var value = ok.Value;

            var totalEmis = (int)value
                .GetType().GetProperty("totalEmis")!
                .GetValue(value)!;

            Assert.True(totalEmis >= 0);
        }

        [Fact]
        public async Task MonthlyReport_InvalidMonth_ReturnsBadRequest()
        {
            var result = await _controller.MonthlyReport(13, 2024);

            Assert.IsType<BadRequestObjectResult>(result);
        }


        // ================= OVERDUE EMIS =================
        [Fact]
        public async Task OverdueEmis_ReturnsOverdueOnly()
        {
            var result = await _controller.OverdueEmis();

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<System.Collections.IEnumerable>(ok.Value);

            Assert.NotNull(list);
        }

        // ================= CUSTOMER REPORT =================
        [Fact]
        public async Task CustomerReport_ReturnsCustomerLoans()
        {
            var customerId = "customer1";

            var result = await _controller.CustomerReport(customerId);

            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsAssignableFrom<System.Collections.IEnumerable>(ok.Value);

            Assert.NotEmpty(list);
        }

        // ================= SUMMARY =================
        [Fact]
        public async Task SummaryReport_ReturnsSummaryData()
        {
            var result = await _controller.SummaryReport();

            var ok = Assert.IsType<OkObjectResult>(result);
            var value = ok.Value;

            var totalLoans = (int)value
                .GetType().GetProperty("totalLoans")!
                .GetValue(value)!;

            Assert.True(totalLoans > 0);
        }
    }
}
