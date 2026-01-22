using Blood_Donation.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blood_Donation.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var stats = new
            {
                TotalDonors = await _context.Donors.CountAsync(),
                TotalPatients = await _context.Patients.CountAsync(),
                TotalUsers = await _context.Users.CountAsync(),
                ActiveDonors = await _context.Donors.CountAsync(d => d.IsAvailable),

                TotalRequests = await _context.BloodRequests.CountAsync(),
                OpenRequests = await _context.BloodRequests.CountAsync(r => r.Status == "Open"),
                ClosedRequests = await _context.BloodRequests.CountAsync(r => r.Status == "Closed"),

                TotalMatches = await _context.DonationMatches.CountAsync(),
                PendingMatches = await _context.DonationMatches.CountAsync(m => m.MatchStatus == "Pending"),
                AcceptedMatches = await _context.DonationMatches.CountAsync(m => m.MatchStatus == "Accepted"),
                CompletedMatches = await _context.DonationMatches.CountAsync(m => m.MatchStatus == "Completed"),

                TotalDonations = await _context.DonationHistories.CountAsync(),
                DonationsThisMonth = await _context.DonationHistories
                    .CountAsync(d => d.DonationDate.Month == DateTime.Now.Month &&
                                     d.DonationDate.Year == DateTime.Now.Year),

                RecentRequests = await _context.BloodRequests
                    .Include(r => r.City)
                    .Include(r => r.Patient)
                    .ThenInclude(p => p.User)
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(5)
                    .ToListAsync(),

                RecentDonations = await _context.DonationHistories
                    .Include(d => d.Donor)
                    .ThenInclude(d => d.User)
                    .Include(d => d.BloodRequest)
                    .OrderByDescending(d => d.DonationDate)
                    .Take(5)
                    .ToListAsync()
            };

            return View(stats);
        }

        // GET: Admin/Users
        public async Task<IActionResult> Users()
        {
            var users = await _context.Users
                .Include(u => u.City)
                .Include(u => u.Donor)
                .Include(u => u.Patient)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            return View(users);
        }

        // GET: Admin/Requests
        public async Task<IActionResult> Requests(string? status)
        {
            var query = _context.BloodRequests
                .Include(r => r.City)
                .Include(r => r.Patient)
                .ThenInclude(p => p.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(r => r.Status == status);
            }

            var requests = await query
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            ViewBag.SelectedStatus = status;
            return View(requests);
        }

        // GET: Admin/Matches
        public async Task<IActionResult> Matches()
        {
            var matches = await _context.DonationMatches
                .Include(m => m.Donor)
                .ThenInclude(d => d.User)
                .Include(m => m.BloodRequest)
                .ThenInclude(r => r.Patient)
                .ThenInclude(p => p.User)
                .OrderByDescending(m => m.MatchedAt)
                .ToListAsync();

            return View(matches);
        }

        // POST: Admin/ToggleUserStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserStatus(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = user.IsActive ? "تم تفعيل المستخدم" : "تم تعطيل المستخدم";
            return RedirectToAction(nameof(Users));
        }

        // POST: Admin/CloseRequest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseRequest(int requestId)
        {
            var request = await _context.BloodRequests.FindAsync(requestId);
            if (request == null)
            {
                return NotFound();
            }

            request.Status = "Closed";
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "تم إغلاق الطلب بنجاح";
            return RedirectToAction(nameof(Requests));
        }
    }
}
