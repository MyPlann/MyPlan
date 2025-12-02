using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyPlan.Data;
using MyPlan.Models;
using MyPlan.Shared;
using MyPlan.ViewModels.FriendVMs;

namespace MyPlan.Controllers
{
    [Authorize(Roles = "visitor")]
    public class FriendController : Controller
    {
        private readonly MyTripDbContext _context;
        private readonly ILogger<FriendController> _logger;

        public FriendController(MyTripDbContext context, ILogger<FriendController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Profile(int id)
        {
            var friend = await _context.Visitors
                .Include(v => v.User)
                .Include(v => v.Highlights)
                .Include(v => v.Bookings)
                    .ThenInclude(b => b.Experience)
                .Include(v => v.Itineraries)
                    .ThenInclude(i => i.Experience)
                .FirstOrDefaultAsync(v => v.VisitorId == id);

            if (friend == null)
            {
                return NotFound();
            }

            // Get current user's upcoming experiences
            var userId = User.GetUserId();
            var currentVisitor = await _context.Visitors.FirstOrDefaultAsync(v => v.UserId == userId);

            var myUpcomingExperiences = await _context.Bookings
                .Include(b => b.Experience)
                .Where(b => b.VisitorId == currentVisitor!.VisitorId
                    && b.BookingStatus == "Confirmed"
                    && b.BookingDate >= DateOnly.FromDateTime(DateTime.Now))
                .Select(b => b.Experience)
                .ToListAsync();

            ViewBag.MyUpcomingExperiences = myUpcomingExperiences;

            return View(friend);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Invite(InviteFriendViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please correct the errors.";
                TempData["OpenInviteModal"] = true;
                return RedirectToAction(nameof(Profile), new { id = model.FriendId });
            }

            try
            {
                var friend = await _context.Visitors
                    .Include(v => v.User)
                    .FirstOrDefaultAsync(v => v.VisitorId == model.FriendId);

                if (friend == null)
                {
                    TempData["Error"] = "Friend not found.";
                    return RedirectToAction(nameof(Profile), new { id = model.FriendId });
                }

                var userId = User.GetUserId();
                var currentVisitor = await _context.Visitors.FirstOrDefaultAsync(v => v.UserId == userId);

                var invitation = new FriendInvitation
                {
                    VisitorId = currentVisitor!.VisitorId,
                    ReceiverId = model.FriendId,
                    ExperienceDetailId = model.ExperienceDetailId,
                    InvitationReceiverEmail = friend.User!.Email,
                    InvitationStatus = "Pending",
                    InvitationToken = Guid.NewGuid().ToString("N"),
                    InvitationSentAt = DateTime.Now,
                    AddedAt = DateTime.Now
                };

                _context.FriendInvitations.Add(invitation);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Invitation sent to {friend.VisitorFirstName} successfully!";
                return RedirectToAction(nameof(Profile), new { id = model.FriendId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending invitation to friend {FriendId}", model.FriendId);
                TempData["Error"] = "Failed to send invitation. Please try again.";
                TempData["OpenInviteModal"] = true;
                return RedirectToAction(nameof(Profile), new { id = model.FriendId });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetExperienceDetails(int experienceId)
        {
            var details = await _context.ExperienceDetails
                .Where(ed => ed.ExperienceId == experienceId
                    && ed.ExperienceDetailDate >= DateOnly.FromDateTime(DateTime.Now))
                .OrderBy(ed => ed.ExperienceDetailDate)
                .Select(ed => new
                {
                    id = ed.ExperienceDetailId,
                    date = ed.ExperienceDetailDate,
                    time = ed.ExperienceDetailTime,
                    price = ed.ExperienceDetailPrice,
                    status = ed.ExperienceDetailStatus
                })
                .ToListAsync();

            return Json(details);
        }
    }
}
