using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankLoanSystem.Models
{
    [Table("Transactions")]
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionId { get; set; }

        [Required]
        public int AccountId { get; set; }

        [Required]
        [StringLength(30)]
        public string TransactionType { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [StringLength(255)]
        public string? Description { get; set; }

        public int? RelatedLoanId { get; set; }

        public int? RelatedDepositId { get; set; }

        public int? CreatedByUserId { get; set; }
    }
}
