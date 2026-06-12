using System.ComponentModel.DataAnnotations;

namespace Quyen15.Models
{
    public class UserAccount
    {
        [Key]
        public int IdUser { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Address { get; set; }

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = "Patient";
        // Patient, Doctor, Staff, Manager

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}