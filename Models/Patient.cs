using System.ComponentModel.DataAnnotations;

namespace Quyen15.Models
{
    public class Patient
    {
        [Key]
        public int IdPatient { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public DateTime Dob { get; set; }

        [StringLength(10)]
        public string Gender { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Address { get; set; }

        public int? IdUser { get; set; }

        public UserAccount? UserAccount { get; set; }

        public List<Appointment> Appointments { get; set; } = new();

        public List<MedicalRecord> MedicalRecords { get; set; } = new();

        public List<DoctorReview> DoctorReviews { get; set; } = new();

        public List<TestResult> TestResults { get; set; } = new();
    }
}