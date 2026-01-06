using LoanManagementSystem.Data;
using LoanManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace LoanManagement.Tests
{
    public static class TestSeedData
    {
        public static void Seed(AppDbContext context)
        {
            // Clear existing data
            context.Users.RemoveRange(context.Users);
            context.LoanTypes.RemoveRange(context.LoanTypes);
            context.LoanApplications.RemoveRange(context.LoanApplications);
            context.EMIs.RemoveRange(context.EMIs);
            context.SaveChanges();

            // ===== Users =====
            var customer = new ApplicationUser
            {
                Id = "customer1",
                UserName = "customer1",
                Email = "customer1@test.com",
                RequestedRole = "Customer"
            };
            var officer = new ApplicationUser
            {
                Id = "officer1",
                UserName = "officer1",
                Email = "officer1@test.com",
                RequestedRole = "LoanOfficer"
            };
            context.Users.AddRange(customer, officer);

            // ===== Loan Types =====
            var personalLoan = new LoanType { LoanTypeId = 1, LoanName = "Personal", ROI = 10, MinAmount = 1000, MaxAmount = 5000, MaxTenure = 24 };
            var homeLoan = new LoanType { LoanTypeId = 2, LoanName = "Home", ROI = 8, MinAmount = 5000, MaxAmount = 50000, MaxTenure = 120 };
            context.LoanTypes.AddRange(personalLoan, homeLoan);

            // ===== Loan Applications =====
            var loan1 = new LoanApplication
            {
                LoanId = 1,
                CustomerId = customer.Id,
                LoanTypeId = personalLoan.LoanTypeId,
                LoanAmount = 2000,
                Tenure = 6,
                Status = LoanStatus.Applied,
                AppliedDate = DateTime.UtcNow,
                User = customer,
                LoanType = personalLoan
            };

            var loan2 = new LoanApplication
            {
                LoanId = 2,
                CustomerId = customer.Id,
                LoanTypeId = homeLoan.LoanTypeId,
                LoanAmount = 6000,
                Tenure = 6,
                Status = LoanStatus.Approved, // Approved so EMIs can be paid
                AppliedDate = DateTime.UtcNow.AddDays(-1),
                User = customer,
                LoanType = homeLoan
            };

            context.LoanApplications.AddRange(loan1, loan2);
            context.SaveChanges();

            // ===== EMIs for loan2 =====
            var emis = new List<EMI>();
            for (int i = 1; i <= loan2.Tenure; i++)
            {
                emis.Add(new EMI
                {
                    LoanId = loan2.LoanId,
                    DueDate = DateTime.UtcNow.AddMonths(i),
                    Amount = 1000, // example fixed EMI
                    PaidStatus = false
                });
            }
            context.EMIs.AddRange(emis);

            context.SaveChanges();
        }
    }
}
