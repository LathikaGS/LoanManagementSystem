using LoanManagementSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace LoanManagementSystem.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<LoanType> LoanTypes { get; set; }
        public DbSet<LoanApplication> LoanApplications { get; set; }
        public DbSet<EMI> EMIs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<LoanApplication>()
                .HasOne(a => a.LoanType)
                .WithMany()
                .HasForeignKey(a => a.LoanTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<LoanApplication>()
                .HasOne(l => l.User)
                .WithMany()
                .HasForeignKey(l => l.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<EMI>()
                .HasOne(e => e.Loan)
                .WithMany(l => l.EMIs)
                .HasForeignKey(e => e.LoanId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<LoanType>()
                .Property(l => l.ROI)
                .HasPrecision(5, 2);

            builder.Entity<EMI>()
                 .Property(e => e.Amount)
                 .HasPrecision(18, 2);

            builder.Entity<LoanApplication>()
                 .Property(e => e.MonthlyIncome)
                  .HasPrecision(18, 2);
        }
    }
}



   


