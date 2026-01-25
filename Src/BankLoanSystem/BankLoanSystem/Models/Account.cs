using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankLoanSystem.Models
{
    [Table("Accounts")]
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AccountId { get; set; }

        [Required]
        public int ClientId { get; set; }

        [Required]
        [StringLength(20)]
        public string AccountNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string AccountType { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; } = 0;

        [StringLength(3)]
        public string Currency { get; set; } = "RUB";

        public DateTime OpeningDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        [Column(TypeName = "decimal(5,2)")]
        public decimal InterestRate { get; set; } = 0;

        [ForeignKey("ClientId")]
        public virtual Client? Client { get; set; }

        public virtual ICollection<Transaction>? Transactions { get; set; }
    }
}
