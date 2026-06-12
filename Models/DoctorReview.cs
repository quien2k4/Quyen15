using System.ComponentModel.DataAnnotations;

namespace Quyen15.Models
{
    public class DoctorReview
    {
        [Key]
        public int IdReview { get; set; }

        [Required]
        public int IdDoctor { get; set; }

        [Required]
        public int IdPatient { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public string? Comment { get; set; }

        public bool IsLike { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public Doctor? Doctor { get; set; }

        public Patient? Patient { get; set; }
    }
}