using LoanManagementSystem.Controllers;
using LoanManagementSystem.Data;
using LoanManagementSystem.DTOs;
using LoanManagementSystem.Models;
using LoanManagementSystem.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Xunit;

namespace LoanManagement.Tests
{
    public class LoanOfficerControllerTests
    {
        private readonly AppDbContext _context;
        private readonly LoanOfficerController _controller;
        private readonly EMICalculator _emi;

        public LoanOfficerControllerTests()
        {
            // Fresh in-memory DB for each test run
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);

            // Seed test data
            TestSeedData.Seed(_context);

            _emi = new EMICalculator();
            _controller = new LoanOfficerController(_context, _emi);

            // Fake LoanOfficer user
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "officer1"),
                new Claim(ClaimTypes.NameIdentifier, "officer1"), 
                new Claim(ClaimTypes.Role, "LoanOfficer")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user}
            };
        }

        [Fact]
        public async Task GetApplications_ReturnsAllLoans()
        {
            var result = await _controller.GetApplications() as OkObjectResult;
            Assert.NotNull(result);

            var loans = result!.Value as IEnumerable<LoanReviewResponseDTO>;
            Assert.NotNull(loans);
            Assert.Equal(2, loans!.Count());
        }

        [Fact]
        public async Task GetByStatus_ReturnsFilteredLoans()
        {
            var result = await _controller.GetByStatus(LoanStatus.Applied) as OkObjectResult;
            Assert.NotNull(result);

            var loans = result!.Value as IEnumerable<LoanReviewResponseDTO>;
            Assert.Single(loans);
            Assert.Equal(LoanStatus.Applied, loans!.First().Status);
        }

        [Fact]
        public async Task MarkUnderReview_UpdatesStatus()
        {
            var dto = new ReviewRemarksDTO { Remarks = "Checking documents" };
            var result = await _controller.MarkUnderReview(1, dto) as OkObjectResult;

            Assert.NotNull(result);
            var loan = result!.Value as LoanReviewResponseDTO;
            Assert.Equal(LoanStatus.UnderReview, loan!.Status);
            Assert.Equal("Checking documents", loan.ReviewRemarks);
        }

        [Fact]
        public async Task ReviewLoan_RejectsLoanWithoutEMI()
        {
            var dto = new ApproveLoanDTO { Status = LoanStatus.Rejected, Remarks = "Incomplete docs" };
            var result = await _controller.ReviewLoan(1, dto) as OkObjectResult;

            Assert.NotNull(result);
            var loan = result!.Value as LoanReviewResponseDTO;
            Assert.Equal(LoanStatus.Rejected, loan!.Status);
            Assert.Equal("Incomplete docs", loan.ReviewRemarks);
        }
    }
}
