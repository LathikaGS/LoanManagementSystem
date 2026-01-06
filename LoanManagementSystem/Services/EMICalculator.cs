namespace LoanManagementSystem.Services
{
    public class EMICalculator
    {
        public decimal Calculate(decimal principal, decimal annualRate, int tenure)
        {
            var r = annualRate / 12;
            r = r / 100;

            var emi = (principal * r *
                (decimal)Math.Pow((double)(1 + r), tenure)) /
                ((decimal)Math.Pow((double)(1 + r), tenure) - 1);

            return Math.Round(emi, 2);
        }
    }
}
