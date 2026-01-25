using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankLoanSystem.Data;
using BankLoanSystem.Models;

namespace BankLoanSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TransactionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            return await _context.Transactions
                .OrderByDescending(t => t.TransactionDate)
                .Take(100)
                .ToListAsync();
        }
    }
}
