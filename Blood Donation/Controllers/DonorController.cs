using Blood_Donation.Infrastructure;
using Blood_Donation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Blood_Donation.Controllers
{
    [Authorize(Roles = "Donor")]
    public class DonorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DonorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Donor/Profile
        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var donor = await _context.Donors
                .Include(d => d.User)
                .ThenInclude(u => u.City)
                .Include(d => d.DonationHistories)
                .ThenInclude(dh => dh.BloodRequest)
                .FirstOrDefaultAsync(d => d.UserId == userId);

            if (donor == null)
            {
                return NotFound();
            }

            return View(donor);
        }

        // GET: Donor/Edit
        public async Task<IActionResult> Edit()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var donor = await _context.Donors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.UserId == userId);

            if (donor == null)
            {
                return NotFound();
            }

            ViewBag.Cities = new SelectList(_context.Cities, "CityId", "CityName", donor.User.CityId);
            return View(donor);
        }

        // POST: Donor/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Donor model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var donor = await _context.Donors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.UserId == userId);

            if (donor == null)
            {
                return NotFound();
            }

            // Update Donor info
            donor.BloodType = model.BloodType;
            donor.Gender = model.Gender;
            donor.DateOfBirth = model.DateOfBirth;
            donor.LastDonationDate = model.LastDonationDate;
            donor.IsAvailable = model.IsAvailable;

            // Update User info
            donor.User.FullName = model.User.FullName;
            donor.User.PhoneNumber = model.User.PhoneNumber;
            donor.User.CityId = model.User.CityId;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "تم تحديث البيانات بنجاح!";
            return RedirectToAction(nameof(Profile));
        }

        // POST: Donor/ToggleAvailability
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAvailability()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var donor = await _context.Donors
                .FirstOrDefaultAsync(d => d.UserId == userId);

            if (donor == null)
            {
                return NotFound();
            }

            donor.IsAvailable = !donor.IsAvailable;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = donor.IsAvailable
                ? "أنت الآن متاح للتبرع!"
                : "تم تعطيل التوفر مؤقتاً";

            return RedirectToAction(nameof(Profile));
        }

        // GET: Donor/MyDonations
        public async Task<IActionResult> MyDonations()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var donations = await _context.DonationHistories
                .Include(dh => dh.BloodRequest)
                .ThenInclude(br => br.Patient)
                .ThenInclude(p => p.User)
                .Where(dh => dh.Donor.UserId == userId)
                .OrderByDescending(dh => dh.DonationDate)
                .ToListAsync();

            return View(donations);
        }
    }
}
