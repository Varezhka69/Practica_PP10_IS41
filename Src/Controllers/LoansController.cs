using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BankLoansApp.Models;

namespace BankLoansApp.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class LoansController : ControllerBase
{
    private readonly AppDbContext _context;

    public LoansController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyLoans()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized("Пользователь не авторизован");
        }

        var loans = await _context.Loans
            .Where(l => l.UserId == userId)
            .ToListAsync();

        return Ok(loans);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateLoan([FromBody] CreateLoanRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized("Пользователь не авторизован");
        }

        var loan = new Loan
        {
            UserId = userId,
            LoanType = request.LoanType,
            Amount = request.Amount,
            InterestRate = request.InterestRate,
            TermMonths = request.TermMonths,
            StartDate = DateTime.UtcNow,
            MonthlyPayment = CalculateMonthlyPayment(request.Amount, request.InterestRate, request.TermMonths),
            RemainingDebt = request.Amount,
            Status = "Active"
        };

        _context.Loans.Add(loan);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "Кредит успешно оформлен",
            LoanId = loan.Id,
            MonthlyPayment = loan.MonthlyPayment
        });
    }

    [HttpGet("details/{id}")]
    public async Task<IActionResult> GetLoanDetails(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized("Пользователь не авторизован");
        }

        var loan = await _context.Loans
            .FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);

        if (loan == null)
        {
            return NotFound("Кредит не найден или у вас нет доступа");
        }

        return Ok(loan);
    }

    [HttpPost("pay/{id}")]
    public async Task<IActionResult> MakePayment(int id, [FromBody] PaymentRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized("Пользователь не авторизован");
        }

        var loan = await _context.Loans
            .FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);

        if (loan == null)
        {
            return NotFound("Кредит не найден");
        }

        if (request.Amount > loan.RemainingDebt)
        {
            return BadRequest("Сумма платежа превышает остаток долга");
        }

        loan.RemainingDebt -= request.Amount;

        if (loan.RemainingDebt <= 0)
        {
            loan.RemainingDebt = 0;
            loan.Status = "Paid";
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "Платеж успешно проведен",
            RemainingDebt = loan.RemainingDebt,
            Status = loan.Status
        });
    }

    private decimal CalculateMonthlyPayment(decimal amount, decimal rate, int term)
    {
        var monthlyRate = rate / 100 / 12;
        var payment = amount * monthlyRate * (decimal)Math.Pow(1 + (double)monthlyRate, term)
                     / (decimal)(Math.Pow(1 + (double)monthlyRate, term) - 1);
        return Math.Round(payment, 2);
    }
}

public class CreateLoanRequest
{
    public string LoanType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal InterestRate { get; set; }
    public int TermMonths { get; set; }
}

public class PaymentRequest
{
    public decimal Amount { get; set; }
}
