using MyPlan.Shared;
using System.ComponentModel.DataAnnotations;

namespace MyPlan.ViewModels.AdminVMs.ReportVMs
{
    // ============ Main Report Index ViewModel ============
    public class ReportIndexViewModel
    {
        // Summary Statistics
        [Display(Name = "Total Revenue")]
        [DataType(DataType.Currency)]
        public decimal TotalRevenue { get; set; }

        [Display(Name = "Total Bookings")]
        public int TotalBookings { get; set; }

        [Display(Name = "Total Users")]
        public int TotalUsers { get; set; }

        [Display(Name = "Total Experiences")]
        public int TotalExperiences { get; set; }

        [Display(Name = "Total Reviews")]
        public int TotalReviews { get; set; }

        [Display(Name = "Total Highlights")]
        public int TotalHighlights { get; set; }

        // Growth Metrics
        [Display(Name = "Revenue Growth")]
        public decimal RevenueGrowth { get; set; }

        [Display(Name = "Booking Growth")]
        public decimal BookingGrowth { get; set; }

        [Display(Name = "User Growth")]
        public decimal UserGrowth { get; set; }

        // Date Range
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Data Collections
        public List<MonthlyRevenueDataViewModel> MonthlyRevenueData { get; set; } = new();
        public List<CategoryDataViewModel> ExperienceCategories { get; set; } = new();
        public List<StatusDataViewModel> BookingStatuses { get; set; } = new();
        public List<RecentActivityViewModel> RecentBookings { get; set; } = new();
        public List<RecentActivityViewModel> RecentPayments { get; set; } = new();
        public List<TopExperienceViewModel> TopExperiences { get; set; } = new();
        public List<UserRegistrationTrendViewModel> UserRegistrations { get; set; } = new();

        // Helper methods
        public string FormatGrowth(decimal growth) => $"{(growth >= 0 ? "+" : "")}{growth:F1}%";
        public string GetGrowthClass(decimal growth) => growth >= 0 ? "text-success" : "text-danger";
        public string GetGrowthIcon(decimal growth) => growth >= 0 ? "bi-arrow-up" : "bi-arrow-down";
    }

    // ============ Generate Report ViewModel ============
    public class GenerateReportViewModel
    {
        [Required(ErrorMessage = "Report type is required")]
        [Display(Name = "Report Type")]
        public string ReportType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Start date is required")]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Now.AddMonths(-1);

        [Required(ErrorMessage = "End date is required")]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; } = DateTime.Now;

        // Validation
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate < StartDate)
            {
                yield return new ValidationResult(
                    "End date must be after or equal to start date",
                    new[] { nameof(EndDate) });
            }

            var validTypes = new[] { "revenue", "bookings", "users", "experiences" };
            if (!validTypes.Contains(ReportType.ToLower()))
            {
                yield return new ValidationResult(
                    "Invalid report type",
                    new[] { nameof(ReportType) });
            }
        }

        public Dictionary<string, string> ReportTypes => new()
        {
            { "revenue", "Revenue Report" },
            { "bookings", "Bookings Report" },
            { "users", "Users Report" },
            { "experiences", "Experiences Report" }
        };
    }

    // ============ Revenue Report ViewModel ============
    public class RevenueReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Display(Name = "Total Revenue")]
        [DataType(DataType.Currency)]
        public decimal TotalRevenue { get; set; }

        [Display(Name = "Average Daily Revenue")]
        [DataType(DataType.Currency)]
        public decimal AverageRevenue { get; set; }

        [Display(Name = "Total Transactions")]
        public int TotalTransactions { get; set; }

        public List<DailyRevenueViewModel> DailyRevenue { get; set; } = new();

        public int TotalDays => (EndDate - StartDate).Days + 1;
    }

    public class DailyRevenueViewModel
    {
        public DateOnly Date { get; set; }

        [DataType(DataType.Currency)]
        public decimal Revenue { get; set; }

        public int TransactionCount { get; set; }

        public string DateFormatted => Date.ToString("MMM dd, yyyy");
    }

    // ============ Bookings Report ViewModel ============
    public class BookingsReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Display(Name = "Total Bookings")]
        public int TotalBookings { get; set; }

        [Display(Name = "Total Revenue")]
        [DataType(DataType.Currency)]
        public decimal TotalRevenue { get; set; }

        [Display(Name = "Total Tickets")]
        public int TotalTickets { get; set; }

        [Display(Name = "Average Bookings Per Day")]
        public double AverageBookingsPerDay { get; set; }

        public List<DailyBookingViewModel> DailyBookings { get; set; } = new();
    }

    public class DailyBookingViewModel
    {
        public DateOnly Date { get; set; }

        public int BookingCount { get; set; }

        [DataType(DataType.Currency)]
        public decimal Revenue { get; set; }

        public int TicketCount { get; set; }

        public string DateFormatted => Date.ToString("MMM dd, yyyy");
    }

    // ============ Users Report ViewModel ============
    public class UsersReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Display(Name = "Total New Users")]
        public int TotalUsers { get; set; }

        [Display(Name = "Average Users Per Day")]
        public double AverageUsersPerDay { get; set; }

        public List<DailyUserViewModel> DailyUsers { get; set; } = new();
    }

    public class DailyUserViewModel
    {
        public DateOnly Date { get; set; }
        public int UserCount { get; set; }
        public string DateFormatted => Date.ToString("MMM dd, yyyy");
    }

    // ============ Experiences Report ViewModel ============
    public class ExperiencesReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Display(Name = "Total Experiences")]
        public int TotalExperiences { get; set; }

        [Display(Name = "Total Revenue")]
        [DataType(DataType.Currency)]
        public decimal TotalRevenue { get; set; }

        [Display(Name = "Total Bookings")]
        public int TotalBookings { get; set; }

        public List<ExperienceReportViewModel> Experiences { get; set; } = new();
    }

    public class ExperienceReportViewModel
    {
        public int ExperienceId { get; set; }

        [Display(Name = "Title")]
        public string? ExperienceTitle { get; set; }

        [Display(Name = "Type")]
        public string? ExperienceType { get; set; }

        [Display(Name = "Location")]
        public string? ExperienceLocation { get; set; }

        [Display(Name = "Bookings")]
        public int BookingCount { get; set; }

        [Display(Name = "Reviews")]
        public int ReviewCount { get; set; }

        [Display(Name = "Average Rating")]
        public double AverageRating { get; set; }

        [Display(Name = "Revenue")]
        [DataType(DataType.Currency)]
        public decimal TotalRevenue { get; set; }

        [Display(Name = "Added At")]
        public DateTime AddedAt { get; set; }

        public string RatingStars => StatusHelper.GetRatingStars((int)Math.Round(AverageRating));
    }

    // ============ Supporting ViewModels ============
    public class MonthlyRevenueDataViewModel
    {
        public int Month { get; set; }

        [DataType(DataType.Currency)]
        public decimal Revenue { get; set; }

        public string MonthName => new DateTime(DateTime.Now.Year, Month, 1).ToString("MMM");
    }

    public class CategoryDataViewModel
    {
        public string Category { get; set; } = string.Empty;
        public int Count { get; set; }

        public string CategoryColor => Category switch
        {
            "Adventure" => "#FF6384",
            "Cultural" => "#36A2EB",
            "Food & Drink" => "#FFCE56",
            "Nature" => "#4BC0C0",
            "Urban" => "#9966FF",
            "Relaxation" => "#FF9F40",
            "Educational" => "#FF6384",
            "Sports" => "#C9CBCF",
            _ => "#E7E9ED"
        };
    }

    public class StatusDataViewModel
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }

        public string StatusColor => Status switch
        {
            StatusHelper.BookingConfirmed => "#28a745",
            StatusHelper.BookingPending => "#ffc107",
            StatusHelper.BookingCancelled => "#dc3545",
            _ => "#6c757d"
        };
    }

    public class RecentActivityViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Type")]
        public string Type { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Amount")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        public string TypeIcon => Type switch
        {
            "Booking" => "bi-calendar-check",
            "Payment" => "bi-cash-coin",
            "Review" => "bi-star",
            "User" => "bi-person-plus",
            _ => "bi-circle"
        };

        public string TypeBadgeClass => Type switch
        {
            "Booking" => "primary",
            "Payment" => "success",
            "Review" => "info",
            "User" => "warning",
            _ => "secondary"
        };

        public string TimeAgo
        {
            get
            {
                var timeSpan = DateTime.Now - Date;

                if (timeSpan.TotalMinutes < 1)
                    return "Just now";
                if (timeSpan.TotalMinutes < 60)
                    return $"{(int)timeSpan.TotalMinutes} min ago";
                if (timeSpan.TotalHours < 24)
                    return $"{(int)timeSpan.TotalHours} hour{((int)timeSpan.TotalHours > 1 ? "s" : "")} ago";
                if (timeSpan.TotalDays < 7)
                    return $"{(int)timeSpan.TotalDays} day{((int)timeSpan.TotalDays > 1 ? "s" : "")} ago";

                return Date.ToString("MMM dd, yyyy");
            }
        }
    }

    public class TopExperienceViewModel
    {
        public int ExperienceId { get; set; }

        [Display(Name = "Experience")]
        public string? ExperienceTitle { get; set; }

        [Display(Name = "Type")]
        public string? ExperienceType { get; set; }

        [Display(Name = "Bookings")]
        public int TotalBookings { get; set; }

        [Display(Name = "Revenue")]
        [DataType(DataType.Currency)]
        public decimal TotalRevenue { get; set; }
    }

    public class UserRegistrationTrendViewModel
    {
        public DateOnly Date { get; set; }
        public int Count { get; set; }
        public string DateFormatted => Date.ToString("MMM dd");
    }

    // ============ Export Options ViewModel ============
    public class ExportReportViewModel
    {
        [Required(ErrorMessage = "Report type is required")]
        public string ReportType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Format is required")]
        [Display(Name = "Export Format")]
        public string Format { get; set; } = "pdf";

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public Dictionary<string, string> Formats => new()
        {
            { "pdf", "PDF Document" },
            { "excel", "Excel Spreadsheet" },
            { "csv", "CSV File" }
        };
    }
}
