using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quyen15.Models;

namespace Quyen15.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("Role");

            List<Doctor> doctors;

            if (userId != null && role == "Patient")
            {
                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.IdUser == userId.Value);

                if (patient != null)
                {
                    doctors = await _context.Doctors
                        .Include(d => d.DoctorReviews)
                        .Where(d => d.Appointments.Any(a =>
                            a.IdPatient == patient.IdPatient &&
                            a.Status == "Completed"))
                        .OrderByDescending(d => d.Appointments.Count(a =>
                            a.IdPatient == patient.IdPatient))
                        .Take(4)
                        .ToListAsync();

                    ViewBag.SectionTitle = "Doctors You Have Visited";

                    if (!doctors.Any())
                    {
                        doctors = await GetPopularDoctors();
                        ViewBag.SectionTitle = "Popular Doctors";
                    }
                }
                else
                {
                    doctors = await GetPopularDoctors();
                    ViewBag.SectionTitle = "Popular Doctors";
                }
            }
            else
            {
                doctors = await GetPopularDoctors();
                ViewBag.SectionTitle = "Popular Doctors";
            }

            return View(doctors);
        }

        private async Task<List<Doctor>> GetPopularDoctors()
        {
            return await _context.Doctors
                .Include(d => d.DoctorReviews)
                .OrderByDescending(d => d.Appointments.Count)
                .ThenBy(d => d.Name)
                .Take(4)
                .ToListAsync();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}