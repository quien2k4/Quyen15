using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quyen15.Models;

namespace Quyen15.Controllers
{
    public class ManagerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManagerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (role != "Manager")
            {
                TempData["Error"] = "Only managers can view this page.";
                return RedirectToAction("Index", "Doctors");
            }

            ViewBag.TotalDoctors = await _context.Doctors.CountAsync();
            ViewBag.TotalPatients = await _context.Patients.CountAsync();
            ViewBag.TotalAppointments = await _context.Appointments.CountAsync();
            ViewBag.PendingAppointments = await _context.Appointments.CountAsync(a => a.Status == "Pending");
            ViewBag.CompletedAppointments = await _context.Appointments.CountAsync(a => a.Status == "Completed");
            ViewBag.CancelledAppointments = await _context.Appointments.CountAsync(a => a.Status == "Cancelled");

            var recentAppointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenByDescending(a => a.AppointmentTime)
                .Take(10)
                .ToListAsync();

            return View(recentAppointments);
        }

        public async Task<IActionResult> Appointments()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (role != "Manager")
            {
                TempData["Error"] = "Only managers can view this page.";
                return RedirectToAction("Index", "Doctors");
            }

            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenByDescending(a => a.AppointmentTime)
                .ToListAsync();

            return View(appointments);
        }

        public async Task<IActionResult> Statistics()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (role != "Manager")
            {
                TempData["Error"] = "Only managers can view this page.";
                return RedirectToAction("Index", "Doctors");
            }

            var doctorStats = await _context.Doctors
                .Select(d => new DoctorStatisticViewModel
                {
                    IdDoctor = d.IdDoctor,
                    DoctorName = d.Name,
                    Specialization = d.Specialization,
                    TotalAppointments = d.Appointments.Count(),
                    PendingAppointments = d.Appointments.Count(a => a.Status == "Pending"),
                    CompletedAppointments = d.Appointments.Count(a => a.Status == "Completed"),
                    CancelledAppointments = d.Appointments.Count(a => a.Status == "Cancelled")
                })
                .OrderByDescending(d => d.TotalAppointments)
                .ToListAsync();

            return View(doctorStats);
        }

        public async Task<IActionResult> ConfirmAppointment(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (role != "Manager")
            {
                TempData["Error"] = "Only managers can confirm appointments.";
                return RedirectToAction("Index", "Doctors");
            }

            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            if (appointment.Status == "Pending")
            {
                appointment.Status = "Confirmed";
                await _context.SaveChangesAsync();
                TempData["Success"] = "Appointment confirmed successfully.";
            }

            return RedirectToAction("Appointments");
        }
    }
}