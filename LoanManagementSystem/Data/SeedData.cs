using LoanManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LoanManagementSystem.Data
{
    public class SeedData
    {
        public static async Task InitializeAsync(
            AppDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            await context.Database.MigrateAsync();

            var roles = new[] { "Admin", "LoanOfficer", "Customer" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // ------------------ ADMIN ------------------
            if (await userManager.FindByEmailAsync("admin@test.com") == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@test.com",
                    EmailConfirmed = true,
                    IsApproved = true
                };

                await userManager.CreateAsync(admin, "Admin123!");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            // ------------------ LOAN OFFICER ------------------
            if (await userManager.FindByEmailAsync("officer@test.com") == null)
            {
                var officer = new ApplicationUser
                {
                    UserName = "officer",
                    Email = "officer@test.com",
                    EmailConfirmed = true,
                    RequestedRole = "LoanOfficer",
                    IsApproved = true
                };

                await userManager.CreateAsync(officer, "Officer123!");
                await userManager.AddToRoleAsync(officer, "LoanOfficer");
            }

            // ------------------ CUSTOMER ------------------
            if (await userManager.FindByEmailAsync("customer@test.com") == null)
            {
                var customer = new ApplicationUser
                {
                    UserName = "customer",
                    Email = "customer@test.com",
                    EmailConfirmed = true,
                    RequestedRole = "Customer",
                    IsApproved = true,
                    AnnualIncome = 600000
                };

                await userManager.CreateAsync(customer, "Customer123!");
                await userManager.AddToRoleAsync(customer, "Customer");
            }

            // ------------------ PENDING USER ------------------
            if (await userManager.FindByEmailAsync("pending@test.com") == null)
            {
                var pendingUser = new ApplicationUser
                {
                    UserName = "pendinguser",
                    Email = "pending@test.com",
                    EmailConfirmed = true,
                    RequestedRole = "LoanOfficer",
                    IsApproved = false,
                    AnnualIncome = 450000
                };

                await userManager.CreateAsync(pendingUser, "Pending123!");
                // ❌ NO ROLE ASSIGNED (Admin must approve)
            }

            // ------------------ LOAN TYPES ------------------
            if (!context.LoanTypes.Any())
            {
                context.LoanTypes.AddRange(
                    new LoanType
                    {
                        LoanName = "Personal Loan",
                        ROI = 12,
                        MaxTenure = 24,
                        MinAmount = 50000,
                        MaxAmount = 500000
                    },
                    new LoanType
                    {
                        LoanName = "Education Loan",
                        ROI = 5,
                        MaxTenure = 60,
                        MinAmount = 100000,
                        MaxAmount = 1500000
                    },
                    new LoanType
                    {
                        LoanName = "Home Loan",
                        ROI = 10,
                        MaxTenure = 120,
                        MinAmount = 500000,
                        MaxAmount = 10000000
                    },
                    new LoanType
                    {
                        LoanName = "Vehicle Loan",
                        ROI = 8,
                        MaxTenure = 48,
                        MinAmount = 50000,
                        MaxAmount = 1500000
                    }
                );

                await context.SaveChangesAsync();
            }

            // ------------------ LOAN APPLICATION ------------------
            if (!context.LoanApplications.Any())
            {
                var customer = await userManager.FindByEmailAsync("customer@test.com");
                var loanType = await context.LoanTypes.FirstAsync();

                if (customer != null && loanType != null)
                {
                    var loan = new LoanApplication
                    {
                        CustomerId = customer.Id,
                        LoanTypeId = loanType.LoanTypeId,
                        LoanAmount = 75000,
                        Tenure = 12,
                        Status = LoanStatus.Approved,
                        AppliedDate = DateTime.UtcNow,
                        EMIs = new List<EMI>()
                    };

                    for (int i = 1; i <= loan.Tenure; i++)
                    {
                        loan.EMIs.Add(new EMI
                        {
                            DueDate = DateTime.UtcNow.AddMonths(i),
                            Amount = 7000,
                            PaidStatus = false
                        });
                    }

                    context.LoanApplications.Add(loan);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
