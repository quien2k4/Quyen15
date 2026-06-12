using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quyen15.Models;

namespace Quyen15.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoctorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search, int page = 1, int pageSize = 3)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (pageSize <= 0)
            {
                pageSize = 3;
            }

            var query = _context.Doctors.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(d =>
                    d.Name.Contains(search) ||
                    d.Specialization.Contains(search) ||
                    (d.Email != null && d.Email.Contains(search))
                );
            }

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            if (totalPages == 0)
            {
                totalPages = 1;
            }

            if (page > totalPages)
            {
                page = totalPages;
            }

            var doctors = await query
                .OrderBy(d => d.IdDoctor)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;

            return View(doctors);
        }

        public async Task<IActionResult> Details(int id)
        {
            var doctor = await _context.Doctors
                .Include(d => d.DoctorReviews)
                .ThenInclude(r => r.Patient)
                .FirstOrDefaultAsync(d => d.IdDoctor == id);

            if (doctor == null)
            {
                return NotFound();
            }

            ViewBag.AverageRating = doctor.DoctorReviews.Any()
                ? doctor.DoctorReviews.Average(r => r.Rating)
                : 0;

            ViewBag.LikeCount = doctor.DoctorReviews.Count(r => r.IsLike);
            ViewBag.ReviewCount = doctor.DoctorReviews.Count;

            return View(doctor);
        }
    }
}