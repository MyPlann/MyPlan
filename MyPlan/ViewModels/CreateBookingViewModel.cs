using System.ComponentModel.DataAnnotations;

namespace MyPlan.ViewModels.BookingVMs
{
    public class CreateBookingViewModel
    {
        [Required(ErrorMessage = "Please select a date and time")]
        public int ExperienceDetailId { get; set; }

        [Required(ErrorMessage = "Number of tickets is required")]
        [Range(1, 10, ErrorMessage = "You can book between 1 and 10 tickets")]
        [Display(Name = "Number of Tickets")]
        public int NumberOfTickets { get; set; }

        [Required(ErrorMessage = "Booking date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Booking Date")]
        public DateOnly BookingDate { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; } = string.Empty;

        public int ExperienceId { get; set; }
    }
}
