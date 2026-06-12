using System.ComponentModel.DataAnnotations;

namespace Quyen15.Models
{
    public class MedicalRecord
    {
        [Key]
        public int IdRecord { get; set; }

        [Required]
        public int IdPatient { get; set; }

        [Required]
        public int IdDoctor { get; set; }

        [Required]
        public int IdAppointment { get; set; }

        [Required]
        public string Diagnosis { get; set; } = string.Empty;

        [Required]
        public string Prescription { get; set; } = string.Empty;

        public string? Notes { get; set; }

        public DateTime RecordDate { get; set; } = DateTime.Now;

        public Patient? Patient { get; set; }

        public Doctor? Doctor { get; set; }

        public Appointment? Appointment { get; set; }
    }
}