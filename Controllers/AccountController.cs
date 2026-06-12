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

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var user = await _context.UserAccounts.FindAsync(userId.Value);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.IdUser == user.IdUser);

            var model = new AccountProfileViewModel
            {
                IdUser = user.IdUser,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role,
                Dob = patient?.Dob,
                Gender = patient?.Gender
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(AccountProfileViewModel model)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var user = await _context.UserAccounts.FindAsync(userId.Value);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            bool emailExists = await _context.UserAccounts
                .AnyAsync(u => u.Email == model.Email && u.IdUser != user.IdUser);

            if (emailExists)
            {
                ModelState.AddModelError("Email", "Email already exists.");
            }

            if (!ModelState.IsValid)
            {
                model.Role = user.Role;
                return View(model);
            }

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.Phone = model.Phone;
            user.Address = model.Address;

            if (user.Role == "Patient")
            {
                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.IdUser == user.IdUser);

                if (patient == null)
                {
                    patient = new Patient
                    {
                        IdUser = user.IdUser
                    };

                    _context.Patients.Add(patient);
                }

                patient.Name = model.FullName;
                patient.Phone = model.Phone;
                patient.Address = model.Address;
                patient.Dob = model.Dob ?? DateTime.Today;
                patient.Gender = string.IsNullOrWhiteSpace(model.Gender) ? "Nam" : model.Gender;
            }

            await _context.SaveChangesAsync();

            HttpContext.Session.SetString("FullName", user.FullName);
            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetString("Phone", user.Phone);
            HttpContext.Session.SetString("Role", user.Role);

            TempData["Success"] = "Profile updated successfully.";

            return RedirectToAction("Profile");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Doctors");
        }
    }
}