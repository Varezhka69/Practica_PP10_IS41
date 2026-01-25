using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankLoanSystem.Models
{
    [Table("Loans")]
    public class Loan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LoanId { get; set; }

        [Required]
        public int ClientId { get; set; }

        [Required]
        [StringLength(20)]
        public string LoanNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string LoanType { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal LoanAmount { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal InterestRate { get; set; }

        [Required]
        public int TermMonths { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MonthlyPayment { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? RemainingAmount { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Активный";

        public int? CreatedByUserId { get; set; }

        [ForeignKey("ClientId")]
        public virtual Client? Client { get; set; }

        public virtual ICollection<Transaction>? Transactions { get; set; }
    }
}
