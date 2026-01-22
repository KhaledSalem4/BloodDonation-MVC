using Blood_Donation.Infrastructure;
using Blood_Donation.Models;
using Blood_Donation.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Blood_Donation.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.Cities = new SelectList(_context.Cities, "CityId", "CityName");
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if email already exists
                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "البريد الإلكتروني مستخدم بالفعل");
                    ViewBag.Cities = new SelectList(_context.Cities, "CityId", "CityName");
                    return View(model);
                }

                // Validate role-specific fields
                if (model.Role == "Donor")
                {
                    if (string.IsNullOrEmpty(model.BloodType) ||
                        string.IsNullOrEmpty(model.Gender) ||
                        model.DateOfBirth == null)
                    {
                        ModelState.AddModelError("", "يرجى إدخال جميع بيانات المتبرع");
                        ViewBag.Cities = new SelectList(_context.Cities, "CityId", "CityName");
                        return View(model);
                    }
                }

                // Create User
                var user = new User
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    PasswordHash = HashPassword(model.Password),
                    PhoneNumber = model.PhoneNumber,
                    CityId = model.CityId,
                    Role = model.Role,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Create Donor or Patient based on role
                if (model.Role == "Donor")
                {
                    var donor = new Donor
                    {
                        UserId = user.UserId,
                        BloodType = model.BloodType!,
                        Gender = model.Gender!,
                        DateOfBirth = model.DateOfBirth!.Value,
                        IsAvailable = true,
                        LastDonationDate = null
                    };
                    _context.Donors.Add(donor);
                }
                else if (model.Role == "Patient")
                {
                    var patient = new Patient
                    {
                        UserId = user.UserId
                    };
                    _context.Patients.Add(patient);
                }

                await _context.SaveChangesAsync();

                // Auto login after registration
                await SignInUser(user);

                TempData["SuccessMessage"] = "تم التسجيل بنجاح!";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Cities = new SelectList(_context.Cities, "CityId", "CityName");
            return View(model);
        }

        // GET: Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user != null && VerifyPassword(model.Password, user.PasswordHash))
                {
                    if (!user.IsActive)
                    {
                        ModelState.AddModelError("", "الحساب غير مفعل");
                        return View(model);
                    }

                    await SignInUser(user, model.RememberMe);

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "البريد الإلكتروني أو كلمة المرور غير صحيحة");
            }

            return View(model);
        }

        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["SuccessMessage"] = "تم تسجيل الخروج بنجاح";
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // Helper Methods
        private async Task SignInUser(User user, bool rememberMe = false)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(2)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private static bool VerifyPassword(string password, string hash)
        {
            var passwordHash = HashPassword(password);
            return passwordHash == hash;
        }
    }
}
