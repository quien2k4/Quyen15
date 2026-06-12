using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quyen15.Models;

namespace Quyen15.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserAccount user, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(user.FullName))
            {
                ModelState.AddModelError("FullName", "Full name is required.");
            }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                ModelState.AddModelError("Email", "Email is required.");
            }

            if (string.IsNullOrWhiteSpace(user.Password))
            {
                ModelState.AddModelError("Password", "Password is required.");
            }

            if (user.Password != confirmPassword)
            {
                ModelState.AddModelError("Password", "Password and confirm password do not match.");
            }

            bool emailExists = await _context.UserAccounts.AnyAsync(u => u.Email == user.Email);
            if (emailExists)
            {
                ModelState.AddModelError("Email", "Email already exists.");
            }

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            user.Role = "Patient";
            user.IsActive = true;
            user.CreatedDate = DateTime.Now;

            _context.UserAccounts.Add(user);
            await _context.SaveChangesAsync();

            var patient = new Patient
            {
                Name = user.FullName,
                Phone = user.Phone,
                Address = user.Address,
                Dob = DateTime.Today,
                Gender = "Nam",
                IdUser = user.IdUser
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Register successfully. Please login.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Please enter email and password.";
                return View();
            }

            var user = await _context.UserAccounts
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password && u.IsActive);

            if (user == null)
            {
                ViewBag.Error = "Email or password is incorrect.";
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.IdUser);
            HttpContext.Session.SetString("FullName", user.FullName);
            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetString("Phone", user.Phone);
            HttpContext.Session.SetString("Role", user.Role);

            return RedirectToAction("Index", "Doctors");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Doctors");
        }
    }
}