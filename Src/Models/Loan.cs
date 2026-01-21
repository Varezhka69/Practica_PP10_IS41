namespace BankLoansApp.Models;

public class Loan
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string LoanType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal InterestRate { get; set; }
    public int TermMonths { get; set; }
    public DateTime StartDate { get; set; }
    public string Status { get; set; } = "Active";
    public decimal MonthlyPayment { get; set; }
    public decimal RemainingDebt { get; set; }
}
