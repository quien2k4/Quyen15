using System.ComponentModel.DataAnnotations;

namespace Quyen15.Models
{
    public class Payment
    {
        [Key]
        public int IdPayment { get; set; }

        [Required]
        public int IdAppointment { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [StringLength(50)]
        public string PaymentMethod { get; set; } = "Online";

        [StringLength(20)]
        public string PaymentStatus { get; set; } = "Paid";
        // Paid, Unpaid, Failed

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        public Appointment? Appointment { get; set; }
    }
}