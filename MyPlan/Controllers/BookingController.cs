using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyPlan.Data;
using MyPlan.ViewModels.AdminVMs.BookingVMs;

namespace MyPlan.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    [Route("Admin/Booking")]
    public class BookingController : Controller
    {
        private readonly MyTripDbContext _context;
        private readonly ILogger<BookingController> _logger;

        public BookingController(MyTripDbContext context, ILogger<BookingController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Display a listing of bookings
        /// </summary>
        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var bookings = await _context.Bookings
                    .Include(b => b.Visitor)
                        .ThenInclude(v => v.User)
                    .Include(b => b.Experience)
                    .Include(b => b.Tickets)
                    .Include(b => b.Payments)
                    .OrderByDescending(b => b.AddedAt)
                    .Select(b => new BookingListViewModel
                    {
                        BookingId = b.BookingId,
                        BookingDate = b.BookingDate,
                        BookingStatus = b.BookingStatus,
                        BookingNumberOfTicket = b.BookingNumberOfTicket ?? 0,
                        TotalAmount = b.TotalAmount ?? 0,
                        VisitorName = b.Visitor.User.FullName,
                        VisitorEmail = b.Visitor.User.Email,
                        ExperienceTitle = b.Experience.ExperienceTitle,
                        ExperienceLocation = b.Experience.ExperienceLocation,
                        TicketCount = b.Tickets.Count,
                        HasPayment = b.Payments.Any(),
                        PaymentStatus = b.Payments.FirstOrDefault() != null ? b.Payments.FirstOrDefault().PaymentStatus : "N/A",
                        AddedAt = b.AddedAt ?? DateTime.Now
                    })
                    .ToListAsync();

                return View("~/Views/Admin/Booking/Index.cshtml", bookings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving bookings list");
                TempData["Error"] = "Failed to load bookings. Please try again.";
                return View(new List<BookingListViewModel>());
            }
        }

        /// <summary>
        /// Update the booking status
        /// </summary>
        [HttpPost("UpdateStatus/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, [FromForm] UpdateBookingStatusViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid booking status.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var booking = await _context.Bookings.FindAsync(id);

                if (booking == null)
                {
                    TempData["Error"] = "Booking not found.";
                    return RedirectToAction(nameof(Index));
                }

                booking.BookingStatus = model.BookingStatus;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Booking status updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking status for booking {BookingId}", id);
                TempData["Error"] = "Failed to update booking status. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Get booking details for modal (AJAX)
        /// </summary>
        [HttpGet("GetBookingDetails")]
        public async Task<IActionResult> GetBookingDetails(int id)
        {
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.Visitor)
                        .ThenInclude(v => v.User)
                    .Include(b => b.Experience)
                    .Include(b => b.Tickets)
                    .Include(b => b.Payments)
                        .ThenInclude(p => p.Booking)
                    .Where(b => b.BookingId == id)
                    .Select(b => new BookingDetailsViewModel
                    {
                        BookingId = b.BookingId,
                        BookingDate = b.BookingDate,
                        BookingStatus = b.BookingStatus,
                        BookingDescription = b.BookingDescription,
                        BookingNumberOfTicket = b.BookingNumberOfTicket ?? 0,
                        BookingPricePerTicket = b.BookingPricePerTicket ?? 0,
                        TotalAmount = b.TotalAmount ?? 0,
                        AddedAt = b.AddedAt ?? DateTime.Now,

                        // Visitor Info
                        VisitorName = b.Visitor.User.FullName,
                        VisitorEmail = b.Visitor.User.Email,
                        VisitorPhone = b.Visitor.VisitorPhoneNumber,
                        VisitorFirstName = b.Visitor.VisitorFirstName,
                        VisitorLastName = b.Visitor.VisitorLastName,

                        // Experience Info
                        ExperienceTitle = b.Experience.ExperienceTitle,
                        ExperienceLocation = b.Experience.ExperienceLocation,
                        ExperienceType = b.Experience.ExperienceType,
                        ExperienceDescription = b.Experience.ExperienceDescription,

                        // Tickets
                        Tickets = b.Tickets.Select(t => new TicketInfoViewModel
                        {
                            TicketId = t.TicketId,
                            TicketCode = t.TicketCode,
                            TicketStatus = t.TicketStatus,
                            TicketType = t.TicketType,
                            TicketSeatNumber = t.TicketSeatNumber,
                            IssuedAt = t.IssuedAt ?? DateTime.Now
                        }).ToList(),

                        // Payment Info
                        Payment = b.Payments.FirstOrDefault() != null ? new PaymentInfoViewModel
                        {
                            PaymentId = b.Payments.First().PaymentId,
                            PaymentDate = b.Payments.First().PaymentDate,
                            PaymentAmount = b.Payments.First().PaymentAmount ?? 0,
                            PaymentMethod = b.Payments.First().PaymentMethod,
                            PaymentStatus = b.Payments.First().PaymentStatus
                        } : null
                    })
                    .FirstOrDefaultAsync();

                if (booking == null)
                {
                    return NotFound();
                }

                return PartialView("_BookingDetailsPartial", booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving booking details for booking {BookingId}", id);
                return StatusCode(500, "Error retrieving booking details");
            }
        }

        /// <summary>
        /// Get invoice details for modal (AJAX)
        /// </summary>
        [HttpGet("GetInvoiceDetails")]
        public async Task<IActionResult> GetInvoiceDetails(int id)
        {
            try
            {
                var invoice = await _context.Invoices
                    .Where(i => i.InvoiceId == id)
                    .Join(_context.Payments,
                        i => i.PaymentId,
                        p => p.PaymentId,
                        (i, p) => new { Invoice = i, Payment = p })
                    .Join(_context.Bookings,
                        x => x.Payment.BookingId,
                        b => b.BookingId,
                        (x, b) => new { x.Invoice, x.Payment, Booking = b })
                    .Join(_context.Visitors,
                        x => x.Booking.VisitorId,
                        v => v.VisitorId,
                        (x, v) => new { x.Invoice, x.Payment, x.Booking, Visitor = v })
                    .Join(_context.Users,
                        x => x.Visitor.UserId,
                        u => u.UserId,
                        (x, u) => new { x.Invoice, x.Payment, x.Booking, x.Visitor, User = u })
                    .Join(_context.Experiences,
                        x => x.Booking.ExperienceId,
                        e => e.ExperienceId,
                        (x, e) => new InvoiceDetailsViewModel
                        {
                            InvoiceId = x.Invoice.InvoiceId,
                            InvoiceDate = x.Invoice.InvoiceDate,
                            InvoiceTotalAmount = x.Invoice.InvoiceTotalAmount ?? 0,
                            TaxAmount = x.Invoice.TaxAmount ?? 0,
                            InvoiceVisitorAddress = x.Invoice.InvoiceVisitorAddress,

                            // Booking Info
                            BookingId = x.Booking.BookingId,
                            BookingDate = x.Booking.BookingDate,
                            BookingNumberOfTicket = x.Booking.BookingNumberOfTicket ?? 0,

                            // Visitor Info
                            VisitorName = x.User.FullName,
                            VisitorEmail = x.User.Email,
                            VisitorPhone = x.Visitor.VisitorPhoneNumber,

                            // Experience Info
                            ExperienceTitle = e.ExperienceTitle,
                            ExperienceLocation = e.ExperienceLocation,

                            // Payment Info
                            PaymentId = x.Payment.PaymentId,
                            PaymentDate = x.Payment.PaymentDate,
                            PaymentAmount = x.Payment.PaymentAmount ?? 0,
                            PaymentMethod = x.Payment.PaymentMethod,
                            PaymentStatus = x.Payment.PaymentStatus
                        })
                    .FirstOrDefaultAsync();

                if (invoice == null)
                {
                    return NotFound();
                }

                return PartialView("_InvoiceDetailsPartial", invoice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoice details for invoice {InvoiceId}", id);
                return StatusCode(500, "Error retrieving invoice details");
            }
        }
    }
}