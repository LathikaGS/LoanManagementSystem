using LoanManagementSystem.Controllers;
using LoanManagementSystem.Data;
using LoanManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class AdminControllerTests
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AdminController _controller;

    public AdminControllerTests()
    {
        // InMemory DB setup
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);

        // Seed users into InMemory DB
        var users = new List<ApplicationUser>
        {
            new ApplicationUser
            {
                Id = "1",
                UserName = "admin",
                Email = "admin@test.com",
                IsApproved = true,
                RequestedRole = "Customer",
                AnnualIncome = 500000
            },
            new ApplicationUser
            {
                Id = "2",
                UserName = "pendingUser",
                Email = "pending@test.com",
                IsApproved = false,
                RequestedRole = "LoanOfficer",
                AnnualIncome = 400000
            }
        };
        _context.Users.AddRange(users);
        _context.SaveChanges();

        // Mock UserManager using InMemory context
        var store = new Mock<IUserStore<ApplicationUser>>();
        var userMgrMock = new Mock<UserManager<ApplicationUser>>(
            store.Object, null, null, null, null, null, null, null, null);

        // Setup FindByIdAsync and GetRolesAsync to work with InMemory users
        userMgrMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((string id) => _context.Users.Find(id));

        userMgrMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync((ApplicationUser u) => new List<string> { u.RequestedRole });

        _userManager = userMgrMock.Object;

        _controller = new AdminController(_context, _userManager);
    }

    // ---------------- APPROVE USER ----------------
    [Fact]
    public async Task ApproveUser_ApprovesAndAssignsRole()
    {
        var user = new ApplicationUser
        {
            Id = "3",
            UserName = "toApprove",
            Email = "approve@test.com",
            IsApproved = false,
            RequestedRole = "LoanOfficer"
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var result = await _controller.ApproveUser("3");

        Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
        Assert.True(user.IsApproved);
    }


    // ---------------- DISABLE USER ----------------
    [Fact]
    public async Task DisableUser_DisablesAccount()
    {
        var user = new ApplicationUser
        {
            Id = "5",
            UserName = "toDisable",
            Email = "disable@test.com",
            RequestedRole = "Customer",
            IsApproved = true
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var result = await _controller.DisableUser("5");

        Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
        Assert.False(user.IsApproved);
    }

    // ---------------- ENABLE USER ----------------
    [Fact]
    public async Task EnableUser_EnablesAccount()
    {
        var user = new ApplicationUser
        {
            Id = "6",
            UserName = "toEnable",
            Email = "enable@test.com",
            RequestedRole = "Customer",
            IsApproved = false
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var result = await _controller.EnableUser("6");

        Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
        Assert.True(user.IsApproved);
    }

    // ---------------- GET APPLICATIONS ----------------
    [Fact]
    public async Task GetAllApplications_ReturnsApplications()
    {
        // Ensure user is linked properly
        var user = new ApplicationUser
        {
            Id = "7",
            UserName = "loanUser",
            Email = "loan@test.com",
            RequestedRole = "Customer",
            IsApproved = true
        };
        _context.Users.Add(user);

        _context.LoanApplications.Add(new LoanApplication
        {
            LoanId = 1,
            LoanAmount = 100000,
            Status = LoanStatus.Applied,
            CustomerId = user.Id,
            User = user,
            LoanType = new LoanType { LoanName = "Home Loan" }
        });

        await _context.SaveChangesAsync();

        var result = await _controller.GetAllApplications();
        Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
    }

    // ---------------- GET LOAN EMIS ----------------
    [Fact]
    public async Task GetLoanEmis_ReturnsEmis()
    {
        _context.EMIs.Add(new EMI
        {
            EMIId = 1,
            LoanId = 1,
            Amount = 5000,
            PaidStatus = false
        });

        await _context.SaveChangesAsync();

        var result = await _controller.GetLoanEmis(1);
        Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
    }
}

  