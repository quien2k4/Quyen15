using System.ComponentModel.DataAnnotations;

namespace Quyen15.Models
{
    public class Appointment
    {
        [Key]
        public int IdAppointment { get; set; }

        [Required]
        public int IdPatient { get; set; }

        [Required]
        public int IdDoctor { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public TimeSpan AppointmentTime { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";
        // Pending, Confirmed, Completed, Cancelled

        public Patient? Patient { get; set; }

        public Doctor? Doctor { get; set; }

        public MedicalRecord? MedicalRecord { get; set; }

        public Payment? Payment { get; set; }

        public List<TestResult> TestResults { get; set; } = new();
    }
}