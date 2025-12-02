using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyPlan.Data;
using MyPlan.Shared;

namespace MyPlan.Controllers
{
    [Authorize(Roles = "visitor")]
    public class CalendarController : Controller
    {
        private readonly MyTripDbContext _context;
        private readonly ILogger<CalendarController> _logger;

        public CalendarController(MyTripDbContext context, ILogger<CalendarController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetCalendarData()
        {
            var userId = User.GetUserId();
            var visitor = await _context.Visitors.FirstOrDefaultAsync(v => v.UserId == userId);

            if (visitor == null)
            {
                return Json(new { error = "Visitor not found" });
            }

            // Get pending invitations
            var pendingInvites = await _context.FriendInvitations
                .Include(fi => fi.Visitor)
                    .ThenInclude(v => v.User)
                .Include(fi => fi.ExperienceDetail)
                    .ThenInclude(ed => ed.Experience)
                .Where(fi => fi.ReceiverId == visitor.VisitorId && fi.InvitationStatus == "Pending")
                .Select(invite => new
                {
                    id = invite.InvitationId,
                    experienceTitle = invite.ExperienceDetail!.Experience!.ExperienceTitle,
                    experienceDescription = invite.ExperienceDetail.Experience.ExperienceDescription,
                    date = invite.ExperienceDetail.ExperienceDetailDate,
                    time = invite.ExperienceDetail.ExperienceDetailTime,
                    location = invite.ExperienceDetail.Experience.ExperienceLocation,
                    price = invite.ExperienceDetail.ExperienceDetailPrice,
                    inviter = new
                    {
                        name = $"{invite.Visitor!.VisitorFirstName} {invite.Visitor.VisitorLastName}",
                        avatar = invite.Visitor.User!.Image
                    }
                })
                .ToListAsync();

            // Get accepted events (bookings)
            var acceptedEvents = await _context.Bookings
                .Include(b => b.Experience)
                .Include(b => b.Visitor)
                    .ThenInclude(v => v.User)
                .Where(b => b.VisitorId == visitor.VisitorId && b.BookingStatus == "Confirmed")
                .Select(booking => new
                {
                    id = booking.BookingId,
                    experienceTitle = booking.Experience!.ExperienceTitle,
                    experienceDescription = booking.Experience.ExperienceDescription,
                    date = booking.BookingDate,
                    location = booking.Experience.ExperienceLocation,
                    ticketNumber = $"MP-{booking.BookingId}",
                    status = "confirmed",
                    price = $"{booking.TotalAmount} SAR"
                })
                .ToListAsync();

            // Get tickets with reviews
            var tickets = await _context.Bookings
                .Include(b => b.Experience)
                .Include(b => b.Tickets)
                .Include(b => b.Reviews)
                .Where(b => b.VisitorId == visitor.VisitorId && b.BookingStatus == "Confirmed")
                .Select(booking => new
                {
                    id = booking.BookingId,
                    experienceId = booking.ExperienceId,
                    experienceTitle = booking.Experience!.ExperienceTitle,
                    date = booking.BookingDate,
                    location = booking.Experience.ExperienceLocation,
                    ticketNumber = $"MP-{booking.BookingId}",
                    price = $"{booking.TotalAmount} SAR",
                    status = booking.BookingStatus,
                    hasReview = booking.Reviews.Any(),
                    reviewId = booking.Reviews.FirstOrDefault() != null ? booking.Reviews.First().ReviewId : (int?)null,
                    reviewRating = booking.Reviews.FirstOrDefault() != null ? booking.Reviews.First().ReviewRating : (int?)null,
                    reviewComment = booking.Reviews.FirstOrDefault() != null ? booking.Reviews.First().ReviewComment : null,
                    tickets = booking.Tickets.Select(t => new
                    {
                        ticketId = t.TicketId,
                        ticketCode = t.TicketCode,
                        ticketStatus = t.TicketStatus,
                        ticketType = t.TicketType,
                        issuedAt = t.IssuedAt
                    })
                })
                .ToListAsync();

            return Json(new
            {
                pendingInvites,
                acceptedEvents,
                tickets,
                currentMonth = DateTime.Now.Month,
                currentYear = DateTime.Now.Year
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HandleInvite(int id, string action)
        {
            try
            {
                var userId = User.GetUserId();
                var visitor = await _context.Visitors.FirstOrDefaultAsync(v => v.UserId == userId);

                var invite = await _context.FriendInvitations
                    .Include(fi => fi.ExperienceDetail)
                    .FirstOrDefaultAsync(fi => fi.InvitationId == id && fi.ReceiverId == visitor!.VisitorId);

                if (invite == null)
                {
                    TempData["Error"] = "Invitation not found.";
                    return RedirectToAction("Show", "Profile");
                }

                if (action == "accept")
                {
                    invite.InvitationStatus = "Accepted";
                    invite.AcceptedAt = DateTime.Now;
                    TempData["Success"] = "Invitation accepted successfully!";
                }
                else if (action == "decline")
                {
                    invite.InvitationStatus = "Rejected";
                    TempData["Success"] = "Invitation declined.";
                }
                else
                {
                    TempData["Error"] = "Invalid action.";
                    return RedirectToAction("Show", "Profile");
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Show", "Profile");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling invitation {InvitationId}", id);
                TempData["Error"] = "An error occurred while processing the invitation.";
                return RedirectToAction("Show", "Profile");
            }
        }
    }

}
