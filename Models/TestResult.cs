using System.ComponentModel.DataAnnotations;

namespace Quyen15.Models
{
    public class TestResult
    {
        [Key]
        public int IdTestResult { get; set; }

        [Required]
        public int IdPatient { get; set; }

        [Required]
        public int IdDoctor { get; set; }

        [Required]
        public int IdAppointment { get; set; }

        [Required]
        [StringLength(100)]
        public string TestType { get; set; } = string.Empty;
        // Blood Test, Ultrasound, X-Ray, ...

        public string? ResultDescription { get; set; }

        [StringLength(255)]
        public string? ResultFile { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public Patient? Patient { get; set; }

        public Doctor? Doctor { get; set; }

        public Appointment? Appointment { get; set; }
    }
}