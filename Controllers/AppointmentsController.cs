using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quyen15.Models;

namespace Quyen15.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int doctorId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null)
            {
                TempData["Error"] = "Please login before registering for examination.";
                return RedirectToAction("Login", "Account");
            }

            if (role != "Patient")
            {
                TempData["Error"] = "Only patients can register for examination.";
                return RedirectToAction("Index", "Doctors");
            }

            var doctor = await _context.Doctors.FindAsync(doctorId);
            if (doctor == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.IdUser == userId.Value);

            if (patient == null)
            {
                TempData["Error"] = "Patient information not found.";
                return RedirectToAction("Index", "Doctors");
            }

            ViewBag.Doctor = doctor;
            ViewBag.Patient = patient;

            var appointment = new Appointment
            {
                IdDoctor = doctor.IdDoctor,
                IdPatient = patient.IdPatient,
                AppointmentDate = DateTime.Today.AddDays(1),
                AppointmentTime = new TimeSpan(8, 0, 0),
                Status = "Pending"
            };

            return View(appointment);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (role != "Patient")
            {
                TempData["Error"] = "Only patients can register for examination.";
                return RedirectToAction("Index", "Doctors");
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.IdUser == userId.Value);

            if (patient == null)
            {
                TempData["Error"] = "Patient information not found.";
                return RedirectToAction("Index", "Doctors");
            }

            var doctor = await _context.Doctors.FindAsync(appointment.IdDoctor);
            if (doctor == null)
            {
                return NotFound();
            }

            appointment.IdPatient = patient.IdPatient;

            if (appointment.AppointmentDate.Date < DateTime.Today)
            {
                ModelState.AddModelError("AppointmentDate", "Appointment date cannot be in the past.");
            }

            bool hasPendingWithSameDoctor = await _context.Appointments.AnyAsync(a =>
                a.IdPatient == patient.IdPatient &&
                a.IdDoctor == appointment.IdDoctor &&
                a.Status == "Pending");

            if (hasPendingWithSameDoctor)
            {
                ModelState.AddModelError("", "You already have a pending appointment with this doctor.");
            }

            bool timeIsBooked = await _context.Appointments.AnyAsync(a =>
                a.IdDoctor == appointment.IdDoctor &&
                a.AppointmentDate.Date == appointment.AppointmentDate.Date &&
                a.AppointmentTime == appointment.AppointmentTime &&
                a.Status != "Cancelled");

            if (timeIsBooked)
            {
                ModelState.AddModelError("", "This doctor already has an appointment at this time.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Doctor = doctor;
                ViewBag.Patient = patient;
                return View(appointment);
            }

            appointment.Status = "Pending";

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Appointment registered successfully.";
            return RedirectToAction("MyAppointments");
        }

        public async Task<IActionResult> MyAppointments()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.IdUser == userId.Value);

            if (patient == null)
            {
                return RedirectToAction("Index", "Doctors");
            }

            var appointments = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.MedicalRecord)
                .Where(a => a.IdPatient == patient.IdPatient)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();

            return View(appointments);
        }

        public async Task<IActionResult> Cancel(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.IdUser == userId.Value);

            if (patient == null)
            {
                return RedirectToAction("Index", "Doctors");
            }

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.IdAppointment == id && a.IdPatient == patient.IdPatient);

            if (appointment == null)
            {
                return NotFound();
            }

            var appointmentDateTime = appointment.AppointmentDate.Date + appointment.AppointmentTime;

            if (appointmentDateTime <= DateTime.Now.AddHours(24))
            {
                TempData["Error"] = "You can only cancel an appointment at least 24 hours before the examination time.";
                return RedirectToAction("MyAppointments");
            }

            appointment.Status = "Cancelled";
            await _context.SaveChangesAsync();

            TempData["Success"] = "Appointment cancelled successfully.";
            return RedirectToAction("MyAppointments");
        }

        public async Task<IActionResult> DoctorAppointments()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (role != "Doctor")
            {
                TempData["Error"] = "Only doctors can view this page.";
                return RedirectToAction("Index", "Doctors");
            }

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.IdUser == userId.Value);

            if (doctor == null)
            {
                TempData["Error"] = "Doctor information not found.";
                return RedirectToAction("Index", "Doctors");
            }

            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.MedicalRecord)
                .Where(a => a.IdDoctor == doctor.IdDoctor)
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToListAsync();

            return View(appointments);
        }
    }
}