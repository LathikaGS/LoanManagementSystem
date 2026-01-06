using LoanManagementSystem.Controllers;
using LoanManagementSystem.DTOs;
using LoanManagementSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class AuthControllerTest
{
    private readonly Mock<UserManager<ApplicationUser>> _userManager;
    private readonly Mock<IConfiguration> _config;
    private readonly AuthController _controller;

    public AuthControllerTest()
    {
        _userManager = MockUserManagerHelper.GetUserManager();
        _config = new Mock<IConfiguration>();

        // Fix JWT key: must be at least 32 chars for HS256
        _config.Setup(x => x["Jwt:Key"])
               .Returns("THIS_IS_A_32_BYTE_LONG_TEST_KEY_!!!");
        _config.Setup(x => x["Jwt:Issuer"]).Returns("TestIssuer");
        _config.Setup(x => x["Jwt:Audience"]).Returns("TestAudience");

        _controller = new AuthController(_userManager.Object, _config.Object);
    }

    // ---------------- REGISTER ----------------
    [Fact]
    public async Task Register_ReturnsOk_WhenValidUser()
    {
        _userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                    .ReturnsAsync((ApplicationUser)null);

        _userManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                    .ReturnsAsync(IdentityResult.Success);

        var dto = new RegisterDTO
        {
            UserName = "user1",
            Email = "user@test.com",
            Password = "Password@123",
            Role = "Customer",
            AnnualIncome = 500000
        };

        var result = await _controller.Register(dto);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenRoleInvalid()
    {
        var dto = new RegisterDTO
        {
            UserName = "user",
            Email = "user@test.com",
            Password = "Password@123",
            Role = "Admin"
        };

        var result = await _controller.Register(dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    // ---------------- LOGIN ----------------
    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenUserNotFound()
    {
        _userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                    .ReturnsAsync((ApplicationUser)null);

        var dto = new LoginDTO
        {
            Email = "wrong@test.com",
            Password = "123"
        };

        var result = await _controller.Login(dto);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenUserNotApproved()
    {
        var user = new ApplicationUser
        {
            Email = "user@test.com",
            IsApproved = false
        };

        _userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                    .ReturnsAsync(user);

        var dto = new LoginDTO
        {
            Email = "user@test.com",
            Password = "Password@123"
        };

        var result = await _controller.Login(dto);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task Login_ReturnsToken_WhenValidUser()
    {
        var user = new ApplicationUser
        {
            Id = "1",
            Email = "user@test.com",
            IsApproved = true
        };

        _userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                    .ReturnsAsync(user);

        _userManager.Setup(x => x.CheckPasswordAsync(user, It.IsAny<string>()))
                    .ReturnsAsync(true);

        _userManager.Setup(x => x.GetRolesAsync(user))
                    .ReturnsAsync(new[] { "Customer" });

        var dto = new LoginDTO
        {
            Email = "user@test.com",
            Password = "Password@123"
        };

        var result = await _controller.Login(dto);

        Assert.IsType<OkObjectResult>(result);
    }

    // ---------------- ME ----------------
    [Fact]
    public async Task Me_ReturnsUserDetails_WhenAuthorized()
    {
        var user = new ApplicationUser
        {
            Id = "1",
            Email = "user@test.com",
            UserName = "user1",
            AnnualIncome = 600000
        };

        _userManager.Setup(x => x.FindByIdAsync("1"))
                    .ReturnsAsync(user);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "1")
                }))
            }
        };

        var result = await _controller.Me();

        Assert.IsType<OkObjectResult>(result);
    }
}
