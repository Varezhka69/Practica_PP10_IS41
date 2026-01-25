using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankLoanSystem.Models
{
    [Table("Clients")]
    public class Client
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClientId { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(50)]
        public string? MiddleName { get; set; }

        [StringLength(4)]
        public string? PassportSeries { get; set; }

        [StringLength(6)]
        public string? PassportNumber { get; set; }

        [Phone]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        [StringLength(255)]
        public string? Address { get; set; }

        public DateTime? BirthDate { get; set; }

        public int CreditRating { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public virtual ICollection<Account>? Accounts { get; set; }
        public virtual ICollection<Loan>? Loans { get; set; }
        public virtual ICollection<Deposit>? Deposits { get; set; }
    }
}
