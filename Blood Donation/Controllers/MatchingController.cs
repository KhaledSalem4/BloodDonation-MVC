using Blood_Donation.Infrastructure;
using Blood_Donation.Models;
using Blood_Donation.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Blood_Donation.Controllers
{
    [Authorize]
    public class MatchingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MatchingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Matching/FindMatches/5 (RequestId)
        [Authorize(Roles = "Patient,Admin")]
        public async Task<IActionResult> FindMatches(int id)
        {
            var request = await _context.BloodRequests
                .Include(br => br.City)
                .Include(br => br.Patient)
                .FirstOrDefaultAsync(br => br.RequestId == id);

            if (request == null)
            {
                return NotFound();
            }

            // Get compatible blood types
            var compatibleTypes = BloodCompatibilityService.GetCompatibleDonorTypes(request.BloodTypeNeeded);

            // Find matching donors
            var matchingDonors = await _context.Donors
                .Include(d => d.User)
                .ThenInclude(u => u.City)
                .Where(d => d.IsAvailable &&
                           d.User.CityId == request.CityId &&
                           compatibleTypes.Contains(d.BloodType))
                .ToListAsync();

            // Filter by last donation date (at least 90 days ago)
            var eligibleDonors = matchingDonors
                .Where(d => !d.LastDonationDate.HasValue ||
                           (DateTime.Now - d.LastDonationDate.Value).Days >= 90)
                .ToList();

            ViewBag.Request = request;
            ViewBag.CompatibleTypes = compatibleTypes;

            return View(eligibleDonors);
        }

        // POST: Matching/CreateMatch
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Patient,Admin")]
        public async Task<IActionResult> CreateMatch(int requestId, int donorId)
        {
            // Check if match already exists
            var existingMatch = await _context.DonationMatches
                .FirstOrDefaultAsync(dm => dm.RequestId == requestId && dm.DonorId == donorId);

            if (existingMatch != null)
            {
                TempData["ErrorMessage"] = "هذا المتبرع تم إضافته مسبقاً";
                return RedirectToAction(nameof(FindMatches), new { id = requestId });
            }

            var match = new DonationMatch
            {
                RequestId = requestId,
                DonorId = donorId,
                MatchStatus = "Pending",
                MatchedAt = DateTime.Now
            };

            _context.DonationMatches.Add(match);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "تم إضافة المتبرع بنجاح! سيتم إشعاره بالطلب.";
            return RedirectToAction(nameof(FindMatches), new { id = requestId });
        }

        // GET: Matching/MyMatches (For Donors)
        [Authorize(Roles = "Donor")]
        public async Task<IActionResult> MyMatches()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var matches = await _context.DonationMatches
                .Include(dm => dm.BloodRequest)
                .ThenInclude(br => br.City)
                .Include(dm => dm.BloodRequest)
                .ThenInclude(br => br.Patient)
                .ThenInclude(p => p.User)
                .Where(dm => dm.Donor.UserId == userId)
                .OrderByDescending(dm => dm.MatchedAt)
                .ToListAsync();

            return View(matches);
        }

        // POST: Matching/RespondToMatch
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Donor")]
        public async Task<IActionResult> RespondToMatch(int matchId, string response)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var match = await _context.DonationMatches
                .Include(dm => dm.Donor)
                .FirstOrDefaultAsync(dm => dm.MatchId == matchId && dm.Donor.UserId == userId);

            if (match == null)
            {
                return NotFound();
            }

            match.MatchStatus = response; // "Accepted" or "Rejected"
            await _context.SaveChangesAsync();

            if (response == "Accepted")
            {
                TempData["SuccessMessage"] = "شكراً لقبولك التبرع! سيتم التواصل معك قريباً.";
            }
            else
            {
                TempData["SuccessMessage"] = "تم رفض الطلب";
            }

            return RedirectToAction(nameof(MyMatches));
        }

        // POST: Matching/ConfirmDonation (Mark donation as completed)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Patient")]
        public async Task<IActionResult> ConfirmDonation(int matchId)
        {
            var match = await _context.DonationMatches
                .Include(dm => dm.Donor)
                .Include(dm => dm.BloodRequest)
                .FirstOrDefaultAsync(dm => dm.MatchId == matchId);

            if (match == null || match.MatchStatus != "Accepted")
            {
                TempData["ErrorMessage"] = "لا يمكن تأكيد هذا التبرع";
                return RedirectToAction("Index", "Home");
            }

            // Create donation history
            var history = new DonationHistory
            {
                DonorId = match.DonorId,
                RequestId = match.RequestId,
                DonationDate = DateTime.Now
            };

            _context.DonationHistories.Add(history);

            // Update donor's last donation date
            match.Donor.LastDonationDate = DateTime.Now;

            // Update match status
            match.MatchStatus = "Completed";

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "تم تسجيل التبرع بنجاح! شكراً لإنقاذ الأرواح 💚";

            return RedirectToAction("Details", "BloodRequest", new { id = match.RequestId });
        }
    }
}
