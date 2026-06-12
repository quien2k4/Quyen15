using System.ComponentModel.DataAnnotations;

namespace Quyen15.Models
{
    public class Doctor
    {
        [Key]
        public int IdDoctor { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Specialization { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(255)]
        public string? Image { get; set; }

        public string? WorkExperience { get; set; }

        public int? IdUser { get; set; }

        public UserAccount? UserAccount { get; set; }

        public List<Appointment> Appointments { get; set; } = new();

        public List<MedicalRecord> MedicalRecords { get; set; } = new();

        public List<DoctorReview> DoctorReviews { get; set; } = new();
    }
}