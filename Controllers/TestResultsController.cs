using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quyen15.Models;

namespace Quyen15.Controllers
{
    public class TestResultsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TestResultsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (role != "Staff")
            {
                TempData["Error"] = "Only staff can view this page.";
                return RedirectToAction("Index", "Doctors");
            }

            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.TestResults)
                .Where(a => a.Status != "Cancelled")
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToListAsync();

            return View(appointments);
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

            if (role != "Staff")
            {
                TempData["Error"] = "Only staff can create test results.";
                return RedirectToAction("Index", "Doctors");
            }

            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.IdAppointment == appointmentId);

            if (appointment == null)
            {
                return NotFound();
            }

            ViewBag.Appointment = appointment;

            var testResult = new TestResult
            {
                IdAppointment = appointment.IdAppointment,
                IdPatient = appointment.IdPatient,
                IdDoctor = appointment.IdDoctor,
                CreatedDate = DateTime.Now
            };

            return View(testResult);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TestResult testResult)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (role != "Staff")
            {
                TempData["Error"] = "Only staff can create test results.";
                return RedirectToAction("Index", "Doctors");
            }

            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.IdAppointment == testResult.IdAppointment);

            if (appointment == null)
            {
                return NotFound();
            }

            testResult.IdPatient = appointment.IdPatient;
            testResult.IdDoctor = appointment.IdDoctor;
            testResult.CreatedDate = DateTime.Now;

            if (!ModelState.IsValid)
            {
                ViewBag.Appointment = appointment;
                return View(testResult);
            }

            _context.TestResults.Add(testResult);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Test result created successfully.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var result = await _context.TestResults
                .Include(t => t.Patient)
                .Include(t => t.Doctor)
                .Include(t => t.Appointment)
                .FirstOrDefaultAsync(t => t.IdTestResult == id);

            if (result == null)
            {
                return NotFound();
            }

            return View(result);
        }
    }
}