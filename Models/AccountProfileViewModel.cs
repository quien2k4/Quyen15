using System.ComponentModel.DataAnnotations;

namespace Quyen15.Models
{
    public class AccountProfileViewModel
    {
        public int IdUser { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Address { get; set; }

        public string Role { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime? Dob { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }
    }
}