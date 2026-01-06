using LoanManagementSystem.Controllers;
using LoanManagementSystem.Data;
using LoanManagementSystem.DTOs;
using LoanManagementSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using Xunit;
using System.Text.Json;

namespace LoanManagement.Tests
{
    public class LoanApplicationControllerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly AppDbContext _context;
        private readonly LoanApplicationController _controller;

        public LoanApplicationControllerTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new AppDbContext(options);

            // Seed LoanTypes
            _context.LoanTypes.Add(new LoanType
            {
                LoanTypeId = 1,
                LoanName = "Personal Loan",
                MinAmount = 1000,
                MaxAmount = 5000,
                MaxTenure = 24
            });
            _context.SaveChanges();

            // Mock UserManager
            var store = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            // Initialize controller
            _controller = new LoanApplicationController(_context, _mockUserManager.Object);

            // Mock user claims
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id")
            }, "mock"));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task ApplyLoan_ReturnsBadRequest_ForInvalidAmount()
        {
            var dto = new ApplyLoanDTO { LoanTypeId = 1, Amount = 0, Tenure = 12 };
            var result = await _controller.ApplyLoan(dto);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);

            // Use JsonDocument to read anonymous object
            var json = JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(badRequest.Value));
            Assert.Equal("INVALID_AMOUNT", json.RootElement.GetProperty("rule").GetString());
        }

        [Fact]
        public async Task ApplyLoan_ReturnsOk_ForValidLoan()
        {
            var dto = new ApplyLoanDTO { LoanTypeId = 1, Amount = 2000, Tenure = 12 };
            var result = await _controller.ApplyLoan(dto);
            var okResult = Assert.IsType<OkObjectResult>(result);

            var json = JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(okResult.Value));
            Assert.True(json.RootElement.GetProperty("loanId").GetInt32() > 0);
        }

        [Fact]
        public async Task GetMyLoans_ReturnsLoans()
        {
            // Seed a loan
            _context.LoanApplications.Add(new LoanApplication
            {
                LoanId = 1,
                CustomerId = "test-user-id",
                LoanTypeId = 1,
                LoanAmount = 2000,
                Tenure = 12,
                Status = LoanStatus.Applied,
                AppliedDate = DateTime.UtcNow
            });
            _context.SaveChanges();

            var result = await _controller.GetMyLoans();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var loans = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            Assert.Single(loans);
        }

        [Fact]
        public async Task GetLoanTypes_ReturnsLoanTypes()
        {
            var result = await _controller.GetLoanTypes();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var loanTypes = Assert.IsAssignableFrom<IEnumerable<LoanType>>(okResult.Value);
            Assert.Single(loanTypes);
        }

        [Fact]
        public async Task GetLoanDetails_ReturnsNotFound_ForInvalidLoan()
        {
            var result = await _controller.GetLoanDetails(999);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetMe_ReturnsUserInfo()
        {
            var user = new ApplicationUser { Id = "test-user-id", UserName = "TestUser", Email = "test@example.com" };
            _mockUserManager.Setup(x => x.FindByIdAsync("test-user-id")).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Customer" });

            var result = await _controller.GetMe();
            var okResult = Assert.IsType<OkObjectResult>(result);

            var json = JsonDocument.Parse(System.Text.Json.JsonSerializer.Serialize(okResult.Value));
            Assert.Equal("TestUser", json.RootElement.GetProperty("name").GetString());
            Assert.Equal("Customer", json.RootElement.GetProperty("role").GetString());
        }
    }
}
