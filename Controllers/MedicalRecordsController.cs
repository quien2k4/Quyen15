using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quyen15.Models;

namespace Quyen15.Controllers
{
    public class MedicalRecordsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MedicalRecordsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Details(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var record = await _context.MedicalRecords
                .Include(m => m.Patient)
                .Include(m => m.Doctor)
                .Include(m => m.Appointment)
                .FirstOrDefaultAsync(m => m.IdRecord == id);

            if (record == null)
            {
                return NotFound();
            }

            if (role == "Patient")
            {
                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.IdUser == userId.Value);

                if (patient == null || record.IdPatient != patient.IdPatient)
                {
                    return RedirectToAction("Index", "Doctors");
                }
            }

            if (role == "Doctor")
            {
                var doctor = await _context.Doctors
                    .FirstOrDefaultAsync(d => d.IdUser == userId.Value);

                if (doctor == null || record.IdDoctor != doctor.IdDoctor)
                {
                    return RedirectToAction("Index", "Doctors");
                }
            }

            return View(record);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int appointmentId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (role != "Doctor")
            {
                TempData["Error"] = "Only doctors can create medical records.";
                return RedirectToAction("Index", "Doctors");
            }

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.IdUser == userId.Value);

            if (doctor == null)
            {
                TempData["Error"] = "Doctor information not found.";
                return RedirectToAction("Index", "Doctors");
            }

            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.MedicalRecord)
                .FirstOrDefaultAsync(a => a.IdAppointment == appointmentId && a.IdDoctor == doctor.IdDoctor);

            if (appointment == null)
            {
                return NotFound();
            }

            if (appointment.Status == "Cancelled")
            {
                TempData["Error"] = "Cannot create medical record for a cancelled appointment.";
                return RedirectToAction("DoctorAppointments", "Appointments");
            }

            if (appointment.MedicalRecord != null)
            {
                TempData["Error"] = "This appointment already has a medical record.";
                return RedirectToAction("DoctorAppointments", "Appointments");
            }

            ViewBag.Appointment = appointment;

            var record = new MedicalRecord
            {
                IdAppointment = appointment.IdAppointment,
                IdPatient = appointment.IdPatient,
                IdDoctor = appointment.IdDoctor,
                RecordDate = DateTime.Now
            };

            return View(record);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MedicalRecord record)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (role != "Doctor")
            {
                TempData["Error"] = "Only doctors can create medical records.";
                return RedirectToAction("Index", "Doctors");
            }

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.IdUser == userId.Value);

            if (doctor == null)
            {
                TempData["Error"] = "Doctor information not found.";
                return RedirectToAction("Index", "Doctors");
            }

            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.MedicalRecord)
                .FirstOrDefaultAsync(a => a.IdAppointment == record.IdAppointment && a.IdDoctor == doctor.IdDoctor);

            if (appointment == null)
            {
                return NotFound();
            }

            if (appointment.MedicalRecord != null)
            {
                TempData["Error"] = "This appointment already has a medical record.";
                return RedirectToAction("DoctorAppointments", "Appointments");
            }

            record.IdPatient = appointment.IdPatient;
            record.IdDoctor = appointment.IdDoctor;
            record.RecordDate = DateTime.Now;

            if (!ModelState.IsValid)
            {
                ViewBag.Appointment = appointment;
                return View(record);
            }

            _context.MedicalRecords.Add(record);

            appointment.Status = "Completed";

            await _context.SaveChangesAsync();

            TempData["Success"] = "Medical record created successfully.";
            return RedirectToAction("DoctorAppointments", "Appointments");
        }
    }
}