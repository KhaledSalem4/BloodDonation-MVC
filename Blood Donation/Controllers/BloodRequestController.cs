using Blood_Donation.Infrastructure;
using Blood_Donation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Blood_Donation.Controllers
{
    [Authorize(Roles = "Patient")]
    public class BloodRequestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BloodRequestController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BloodRequest/Create
        public IActionResult Create()
        {
            ViewBag.Cities = new SelectList(_context.Cities, "CityId", "CityName");
            return View();
        }

        // POST: BloodRequest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BloodRequest model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (patient == null)
            {
                TempData["ErrorMessage"] = "خطأ: لم يتم العثور على بيانات المريض";
                return RedirectToAction("Index", "Home");
            }

            model.PatientId = patient.PatientId;
            model.Status = "Open";
            model.CreatedAt = DateTime.Now;

            _context.BloodRequests.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "تم إنشاء الطلب بنجاح! سيتم البحث عن متبرعين متوافقين.";
            return RedirectToAction(nameof(MyRequests));
        }

        // GET: BloodRequest/MyRequests
        public async Task<IActionResult> MyRequests()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var requests = await _context.BloodRequests
                .Include(br => br.City)
                .Include(br => br.DonationMatches)
                .ThenInclude(dm => dm.Donor)
                .ThenInclude(d => d.User)
                .Where(br => br.Patient.UserId == userId)
                .OrderByDescending(br => br.CreatedAt)
                .ToListAsync();

            return View(requests);
        }

        // GET: BloodRequest/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var request = await _context.BloodRequests
                .Include(br => br.City)
                .Include(br => br.Patient)
                .ThenInclude(p => p.User)
                .Include(br => br.DonationMatches)
                .ThenInclude(dm => dm.Donor)
                .ThenInclude(d => d.User)
                .ThenInclude(u => u.City)
                .FirstOrDefaultAsync(br => br.RequestId == id && br.Patient.UserId == userId);

            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        // POST: BloodRequest/Close/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Close(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var request = await _context.BloodRequests
                .FirstOrDefaultAsync(br => br.RequestId == id && br.Patient.UserId == userId);

            if (request == null)
            {
                return NotFound();
            }

            request.Status = "Closed";
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "تم إغلاق الطلب بنجاح";
            return RedirectToAction(nameof(MyRequests));
        }

        // GET: BloodRequest/AllRequests (For Donors to see available requests)
        [AllowAnonymous]
        [Authorize(Roles = "Donor,Admin")]
        public async Task<IActionResult> AllRequests(string? bloodType, int? cityId)
        {
            var query = _context.BloodRequests
                .Include(br => br.City)
                .Include(br => br.Patient)
                .ThenInclude(p => p.User)
                .Where(br => br.Status == "Open");

            if (!string.IsNullOrEmpty(bloodType))
            {
                query = query.Where(br => br.BloodTypeNeeded == bloodType);
            }

            if (cityId.HasValue)
            {
                query = query.Where(br => br.CityId == cityId.Value);
            }

            var requests = await query
                .OrderByDescending(br => br.UrgencyLevel == "High")
                .ThenByDescending(br => br.CreatedAt)
                .ToListAsync();

            ViewBag.Cities = new SelectList(_context.Cities, "CityId", "CityName");
            ViewBag.SelectedBloodType = bloodType;
            ViewBag.SelectedCityId = cityId;

            return View(requests);
        }

        // POST: BloodRequest/RequestToDonate
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Donor")]
        public async Task<IActionResult> RequestToDonate(int requestId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var donor = await _context.Donors
                .FirstOrDefaultAsync(d => d.UserId == userId);

            if (donor == null)
            {
                TempData["ErrorMessage"] = "خطأ: لم يتم العثور على بيانات المتبرع";
                return RedirectToAction(nameof(AllRequests));
            }

            var request = await _context.BloodRequests
                .FirstOrDefaultAsync(br => br.RequestId == requestId && br.Status == "Open");

            if (request == null)
            {
                TempData["ErrorMessage"] = "عذراً، هذا الطلب غير متاح";
                return RedirectToAction(nameof(AllRequests));
            }

            // Check if donor already requested to donate for this request
            var existingMatch = await _context.DonationMatches
                .FirstOrDefaultAsync(dm => dm.RequestId == requestId && dm.DonorId == donor.DonorId);

            if (existingMatch != null)
            {
                TempData["ErrorMessage"] = "لقد قمت بالفعل بطلب التبرع لهذا الطلب";
                return RedirectToAction(nameof(AllRequests));
            }

            // Create donation match with Pending status
            var donationMatch = new DonationMatch
            {
                RequestId = requestId,
                DonorId = donor.DonorId,
                MatchStatus = "Pending",
                MatchedAt = DateTime.Now
            };

            _context.DonationMatches.Add(donationMatch);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "تم إرسال طلب التبرع بنجاح! سيتم التواصل معك من قبل المريض";
            return RedirectToAction(nameof(AllRequests));
        }

        // POST: BloodRequest/AcceptDonor
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> AcceptDonor(int matchId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var match = await _context.DonationMatches
                .Include(dm => dm.BloodRequest)
                .ThenInclude(br => br.Patient)
                .Include(dm => dm.Donor)
                .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(dm => dm.MatchId == matchId && dm.BloodRequest.Patient.UserId == userId);

            if (match == null)
            {
                TempData["ErrorMessage"] = "عذراً، لم يتم العثور على هذا الطلب";
                return RedirectToAction(nameof(MyRequests));
            }

            match.MatchStatus = "Accepted";
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"تم قبول المتبرع {match.Donor?.User?.FullName} بنجاح! يمكنك التواصل معه على رقم: {match.Donor?.User?.PhoneNumber}";
            return RedirectToAction(nameof(Details), new { id = match.RequestId });
        }

        // POST: BloodRequest/RejectDonor
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> RejectDonor(int matchId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var match = await _context.DonationMatches
                .Include(dm => dm.BloodRequest)
                .ThenInclude(br => br.Patient)
                .FirstOrDefaultAsync(dm => dm.MatchId == matchId && dm.BloodRequest.Patient.UserId == userId);

            if (match == null)
            {
                TempData["ErrorMessage"] = "عذراً، لم يتم العثور على هذا الطلب";
                return RedirectToAction(nameof(MyRequests));
            }

            match.MatchStatus = "Rejected";
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "تم رفض المتبرع";
            return RedirectToAction(nameof(Details), new { id = match.RequestId });
        }
    }
}
