using LoanManagementSystem.Controllers;
using LoanManagementSystem.Data;
using LoanManagementSystem.DTOs;
using LoanManagementSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Xunit;

namespace LoanManagement.Tests
{
    public class LoanTypeControllerTests
    {
        private readonly AppDbContext _context;
        private readonly LoanTypeController _controller;

        public LoanTypeControllerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);

            // Seed loan types
            _context.LoanTypes.AddRange(
                new LoanType { LoanTypeId = 1, LoanName = "Personal", ROI = 10, MinAmount = 1000, MaxAmount = 5000, MaxTenure = 24 },
                new LoanType { LoanTypeId = 2, LoanName = "Home", ROI = 8, MinAmount = 5000, MaxAmount = 50000, MaxTenure = 120 }
            );
            _context.SaveChanges();

            _controller = new LoanTypeController(_context);

            // Fake Admin user
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task GetLoanTypes_ReturnsAllLoanTypes()
        {
            var result = await _controller.GetLoanTypes() as OkObjectResult;
            Assert.NotNull(result);

            var loanTypes = result!.Value as IEnumerable<LoanType>;
            Assert.NotNull(loanTypes);
            Assert.Equal(2, loanTypes!.Count());
        }

        [Fact]
        public async Task AddLoanType_AddsNewLoanType()
        {
            var dto = new LoanTypeDTO
            {
                LoanName = "Car",
                ROI = 12,
                MinAmount = 5000,
                MaxAmount = 20000,
                MaxTenure = 60
            };

            var result = await _controller.AddLoanType(dto) as OkObjectResult;
            Assert.NotNull(result);

            var addedLoanType = result!.Value as LoanType;
            Assert.NotNull(addedLoanType);
            Assert.Equal("Car", addedLoanType!.LoanName);
            Assert.Equal(12, addedLoanType.ROI);
        }

        [Fact]
        public async Task UpdateLoanType_UpdatesExistingLoanType()
        {
            var dto = new LoanTypeDTO
            {
                LoanName = "Updated Personal",
                ROI = 11,
                MinAmount = 1500,
                MaxAmount = 6000,
                MaxTenure = 36
            };

            var result = await _controller.UpdateLoanType(1, dto) as OkObjectResult;
            Assert.NotNull(result);

            var updatedLoanType = result!.Value as LoanType;
            Assert.NotNull(updatedLoanType);
            Assert.Equal("Updated Personal", updatedLoanType!.LoanName);
            Assert.Equal(11, updatedLoanType.ROI);
        }
    }
}
