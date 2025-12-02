using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyPlan.Data;
using MyPlan.Shared;
using QRCoder;
using System.Drawing.Imaging;

namespace MyPlan.Controllers
{
    [Authorize(Roles = "visitor")]
    public class TicketController : Controller
    {
        private readonly MyTripDbContext _context;
        private readonly ILogger<TicketController> _logger;

        public TicketController(MyTripDbContext context, ILogger<TicketController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Download(int id)
        {
            try
            {
                var userId = User.GetUserId();
                var visitor = await _context.Visitors.FirstOrDefaultAsync(v => v.UserId == userId);

                var booking = await _context.Bookings
                    .Include(b => b.Experience)
                    .Include(b => b.Tickets)
                    .Include(b => b.Visitor)
                        .ThenInclude(v => v.User)
                    .FirstOrDefaultAsync(b => b.BookingId == id && b.VisitorId == visitor!.VisitorId);

                if (booking == null)
                {
                    return NotFound();
                }

                // Generate QR Code
                var ticketCode = booking.Tickets.FirstOrDefault()?.TicketCode ?? booking.BookingId.ToString();
                var qrCodeBase64 = GenerateQRCode(ticketCode);

                ViewBag.QRCode = qrCodeBase64;
                ViewBag.Booking = booking;
                ViewBag.User = booking.Visitor?.User;

                // For PDF generation, you would use a library like DinkToPdf or IronPdf
                // Return view for now
                return View("TicketPdf", booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading ticket {BookingId}", id);
                TempData["Error"] = "Error generating ticket.";
                return RedirectToAction("Show", "Profile");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var userId = User.GetUserId();
                var visitor = await _context.Visitors.FirstOrDefaultAsync(v => v.UserId == userId);

                var booking = await _context.Bookings
                    .Include(b => b.Experience)
                    .FirstOrDefaultAsync(b => b.BookingId == id && b.VisitorId == visitor!.VisitorId);

                if (booking == null)
                {
                    TempData["Error"] = "Booking not found.";
                    return RedirectToAction("Show", "Profile");
                }

                // Check if cancellation is allowed (at least 24 hours before event)
                var eventDate = booking.Experience?.ExperienceStartDate;
                if (eventDate.HasValue)
                {
                    var eventDateTime = eventDate.Value.ToDateTime(TimeOnly.MinValue);
                    var hoursUntilEvent = (eventDateTime - DateTime.Now).TotalHours;

                    if (hoursUntilEvent < 24)
                    {
                        TempData["Error"] = "Cancellation is only allowed up to 24 hours before the event.";
                        return RedirectToAction("Show", "Profile");
                    }
                }

                // Update booking status
                booking.BookingStatus = "Cancelled";

                // Update tickets status
                var tickets = await _context.Tickets.Where(t => t.BookingId == id).ToListAsync();
                foreach (var ticket in tickets)
                {
                    ticket.TicketStatus = "Cancelled";
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = "Booking cancelled successfully!";
                return RedirectToAction("Show", "Profile");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling booking {BookingId}", id);
                TempData["Error"] = "An error occurred while cancelling the booking.";
                return RedirectToAction("Show", "Profile");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> View(string ticketCode)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Booking)
                    .ThenInclude(b => b.Experience)
                .Include(t => t.Booking)
                    .ThenInclude(b => b.Visitor)
                        .ThenInclude(v => v.User)
                .FirstOrDefaultAsync(t => t.TicketCode == ticketCode);

            if (ticket == null)
            {
                return NotFound();
            }

            var qrCodeBase64 = GenerateQRCode(ticket.TicketCode ?? string.Empty);
            ViewBag.QRCode = qrCodeBase64;

            return View(ticket);
        }

        private string GenerateQRCode(string text)
        {
            using var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new QRCode(qrCodeData);
            using var qrCodeImage = qrCode.GetGraphic(20);

            using var ms = new MemoryStream();
            qrCodeImage.Save(ms, ImageFormat.Png);
            var byteImage = ms.ToArray();
            return Convert.ToBase64String(byteImage);
        }
    }

}
