namespace Quyen15.Models
{
    public class DoctorStatisticViewModel
    {
        public int IdDoctor { get; set; }

        public string DoctorName { get; set; } = string.Empty;

        public string Specialization { get; set; } = string.Empty;

        public int TotalAppointments { get; set; }

        public int PendingAppointments { get; set; }

        public int CompletedAppointments { get; set; }

        public int CancelledAppointments { get; set; }
    }
}