using MyPlan.Shared;
using System.ComponentModel.DataAnnotations;

namespace MyPlan.ViewModels.AdminVMs.BookingVMs
{
    // ============ List ViewModel ============
    public class BookingListViewModel
    {
        public int BookingId { get; set; }

        [Display(Name = "Booking Date")]
        public DateOnly BookingDate { get; set; }

        [Display(Name = "Status")]
        public string? BookingStatus { get; set; }

        [Display(Name = "Number of Tickets")]
        public int BookingNumberOfTicket { get; set; }

        [Display(Name = "Total Amount")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Visitor Name")]
        public string? VisitorName { get; set; }

        [Display(Name = "Visitor Email")]
        public string? VisitorEmail { get; set; }

        [Display(Name = "Experience")]
        public string? ExperienceTitle { get; set; }

        [Display(Name = "Location")]
        public string? ExperienceLocation { get; set; }

        public int TicketCount { get; set; }
        public bool HasPayment { get; set; }

        [Display(Name = "Payment Status")]
        public string? PaymentStatus { get; set; }

        [Display(Name = "Added At")]
        public DateTime AddedAt { get; set; }

        // Helper properties for display
        public string StatusBadgeClass => BookingStatus switch
        {
            StatusHelper.BookingConfirmed => "success",
            StatusHelper.BookingPending => "warning",
            StatusHelper.BookingCancelled => "danger",
            _ => "secondary"
        };

        public string PaymentStatusBadgeClass => PaymentStatus switch
        {
            StatusHelper.PaymentPaid => "success",
            StatusHelper.PaymentPending => "warning",
            StatusHelper.PaymentFailed => "danger",
            _ => "secondary"
        };
    }

    // ============ Update Status ViewModel ============
    public class UpdateBookingStatusViewModel
    {
        [Required(ErrorMessage = "Booking status is required")]
        [Display(Name = "Booking Status")]
        public string BookingStatus { get; set; } = string.Empty;

        // Custom validation
        public bool IsValidStatus()
        {
            var validStatuses = new[]
            {
                StatusHelper.BookingPending,
                StatusHelper.BookingConfirmed,
                StatusHelper.BookingCancelled
            };
            return validStatuses.Contains(BookingStatus);
        }
    }

    // ============ Booking Details ViewModel ============
    public class BookingDetailsViewModel
    {
        // Booking Info
        public int BookingId { get; set; }

        [Display(Name = "Booking Date")]
        public DateOnly BookingDate { get; set; }

        [Display(Name = "Status")]
        public string? BookingStatus { get; set; }

        [Display(Name = "Description")]
        public string? BookingDescription { get; set; }

        [Display(Name = "Number of Tickets")]
        public int BookingNumberOfTicket { get; set; }

        [Display(Name = "Price Per Ticket")]
        [DataType(DataType.Currency)]
        public decimal BookingPricePerTicket { get; set; }

        [Display(Name = "Total Amount")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Added At")]
        public DateTime AddedAt { get; set; }

        // Visitor Info
        [Display(Name = "Visitor Name")]
        public string? VisitorName { get; set; }

        [Display(Name = "Email")]
        public string? VisitorEmail { get; set; }

        [Display(Name = "Phone")]
        public string? VisitorPhone { get; set; }

        public string? VisitorFirstName { get; set; }
        public string? VisitorLastName { get; set; }

        // Experience Info
        [Display(Name = "Experience")]
        public string? ExperienceTitle { get; set; }

        [Display(Name = "Location")]
        public string? ExperienceLocation { get; set; }

        [Display(Name = "Type")]
        public string? ExperienceType { get; set; }

        [Display(Name = "Description")]
        public string? ExperienceDescription { get; set; }

        // Related Data
        public List<TicketInfoViewModel> Tickets { get; set; } = new();
        public PaymentInfoViewModel? Payment { get; set; }

        // Helper properties
        public string StatusBadgeClass => BookingStatus switch
        {
            StatusHelper.BookingConfirmed => "success",
            StatusHelper.BookingPending => "warning",
            StatusHelper.BookingCancelled => "danger",
            _ => "secondary"
        };
    }

    // ============ Ticket Info ViewModel ============
    public class TicketInfoViewModel
    {
        public int TicketId { get; set; }

        [Display(Name = "Ticket Code")]
        public string? TicketCode { get; set; }

        [Display(Name = "Status")]
        public string? TicketStatus { get; set; }

        [Display(Name = "Type")]
        public string? TicketType { get; set; }

        [Display(Name = "Seat Number")]
        public string? TicketSeatNumber { get; set; }

        [Display(Name = "Issued At")]
        public DateTime IssuedAt { get; set; }

        public string StatusBadgeClass => TicketStatus switch
        {
            StatusHelper.TicketValid => "success",
            StatusHelper.TicketUsed => "info",
            StatusHelper.TicketExpired => "warning",
            StatusHelper.TicketCancelled => "danger",
            _ => "secondary"
        };
    }

    // ============ Payment Info ViewModel ============
    public class PaymentInfoViewModel
    {
        public int PaymentId { get; set; }

        [Display(Name = "Payment Date")]
        public DateOnly? PaymentDate { get; set; }

        [Display(Name = "Amount")]
        [DataType(DataType.Currency)]
        public decimal PaymentAmount { get; set; }

        [Display(Name = "Method")]
        public string? PaymentMethod { get; set; }

        [Display(Name = "Status")]
        public string? PaymentStatus { get; set; }

        public string StatusBadgeClass => PaymentStatus switch
        {
            StatusHelper.PaymentPaid => "success",
            StatusHelper.PaymentPending => "warning",
            StatusHelper.PaymentFailed => "danger",
            _ => "secondary"
        };

        public string MethodIcon => PaymentMethod switch
        {
            StatusHelper.PaymentMethodCard => "bi-credit-card",
            StatusHelper.PaymentMethodPayPal => "bi-paypal",
            _ => "bi-cash"
        };
    }

    // ============ Invoice Details ViewModel ============
    public class InvoiceDetailsViewModel
    {
        // Invoice Info
        public int InvoiceId { get; set; }

        [Display(Name = "Invoice Date")]
        public DateOnly? InvoiceDate { get; set; }

        [Display(Name = "Total Amount")]
        [DataType(DataType.Currency)]
        public decimal InvoiceTotalAmount { get; set; }

        [Display(Name = "Tax Amount")]
        [DataType(DataType.Currency)]
        public decimal TaxAmount { get; set; }

        [Display(Name = "Visitor Address")]
        public string? InvoiceVisitorAddress { get; set; }

        // Booking Info
        public int BookingId { get; set; }

        [Display(Name = "Booking Date")]
        public DateOnly BookingDate { get; set; }

        [Display(Name = "Number of Tickets")]
        public int BookingNumberOfTicket { get; set; }

        // Visitor Info
        [Display(Name = "Visitor Name")]
        public string? VisitorName { get; set; }

        [Display(Name = "Email")]
        public string? VisitorEmail { get; set; }

        [Display(Name = "Phone")]
        public string? VisitorPhone { get; set; }

        // Experience Info
        [Display(Name = "Experience")]
        public string? ExperienceTitle { get; set; }

        [Display(Name = "Location")]
        public string? ExperienceLocation { get; set; }

        // Payment Info
        public int PaymentId { get; set; }

        [Display(Name = "Payment Date")]
        public DateOnly? PaymentDate { get; set; }

        [Display(Name = "Payment Amount")]
        [DataType(DataType.Currency)]
        public decimal PaymentAmount { get; set; }

        [Display(Name = "Payment Method")]
        public string? PaymentMethod { get; set; }

        [Display(Name = "Payment Status")]
        public string? PaymentStatus { get; set; }

        // Calculated properties
        public decimal SubTotal => InvoiceTotalAmount - TaxAmount;

        public string PaymentMethodIcon => PaymentMethod switch
        {
            StatusHelper.PaymentMethodCard => "bi-credit-card",
            StatusHelper.PaymentMethodPayPal => "bi-paypal",
            _ => "bi-cash"
        };
    }
}
