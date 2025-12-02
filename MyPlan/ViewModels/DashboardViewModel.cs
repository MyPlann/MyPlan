using MyPlan.Shared;
using System.ComponentModel.DataAnnotations;

namespace MyPlan.ViewModels.AdminVMs.DashboardVMs
{
    public class DashboardViewModel
    {
        // Summary Statistics
        [Display(Name = "Total Revenue")]
        [DataType(DataType.Currency)]
        public decimal TotalRevenue { get; set; }

        [Display(Name = "Total Events")]
        public int TotalEvents { get; set; }

        [Display(Name = "Total Users")]
        public int TotalUsers { get; set; }

        [Display(Name = "Active Bookings")]
        public int ActiveBookings { get; set; }

        // Growth Metrics
        [Display(Name = "Revenue Growth")]
        public decimal RevenueGrowth { get; set; }

        [Display(Name = "Booking Growth")]
        public decimal BookingGrowth { get; set; }

        [Display(Name = "User Growth")]
        public decimal UserGrowth { get; set; }

        // Lists
        public List<RecentBookingViewModel> RecentBookings { get; set; } = new();
        public List<MonthlyRevenueViewModel> RevenueData { get; set; } = new();
        public List<EventCategoryViewModel> EventCategories { get; set; } = new();

        // Helper methods for formatting
        public string FormatGrowth(decimal growth)
        {
            var sign = growth >= 0 ? "+" : "";
            return $"{sign}{growth:F1}%";
        }

        public string GetGrowthClass(decimal growth)
        {
            return growth >= 0 ? "text-success" : "text-danger";
        }

        public string GetGrowthIcon(decimal growth)
        {
            return growth >= 0 ? "bi-arrow-up" : "bi-arrow-down";
        }
    }

    // ============ Recent Booking ViewModel ============
    public class RecentBookingViewModel
    {
        public int BookingId { get; set; }

        [Display(Name = "Booking Date")]
        public DateOnly BookingDate { get; set; }

        [Display(Name = "Status")]
        public string? BookingStatus { get; set; }

        [Display(Name = "Total Amount")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Visitor")]
        public string? VisitorName { get; set; }

        [Display(Name = "Email")]
        public string? VisitorEmail { get; set; }

        [Display(Name = "Experience")]
        public string? ExperienceTitle { get; set; }

        [Display(Name = "Added")]
        public DateTime AddedAt { get; set; }

        // Helper properties
        public string StatusBadgeClass => BookingStatus switch
        {
            StatusHelper.BookingConfirmed => "success",
            StatusHelper.BookingPending => "warning",
            StatusHelper.BookingCancelled => "danger",
            _ => "secondary"
        };

        public string TimeAgo
        {
            get
            {
                var timeSpan = DateTime.Now - AddedAt;

                if (timeSpan.TotalMinutes < 1)
                    return "Just now";
                if (timeSpan.TotalMinutes < 60)
                    return $"{(int)timeSpan.TotalMinutes} min ago";
                if (timeSpan.TotalHours < 24)
                    return $"{(int)timeSpan.TotalHours} hour{((int)timeSpan.TotalHours > 1 ? "s" : "")} ago";
                if (timeSpan.TotalDays < 7)
                    return $"{(int)timeSpan.TotalDays} day{((int)timeSpan.TotalDays > 1 ? "s" : "")} ago";

                return AddedAt.ToString("MMM dd, yyyy");
            }
        }
    }

    // ============ Monthly Revenue ViewModel ============
    public class MonthlyRevenueViewModel
    {
        [Display(Name = "Year")]
        public int Year { get; set; }

        [Display(Name = "Month")]
        public int Month { get; set; }

        [Display(Name = "Revenue")]
        [DataType(DataType.Currency)]
        public decimal Revenue { get; set; }

        // Helper properties
        public string MonthName => new DateTime(Year, Month, 1).ToString("MMM");
        public string MonthYear => new DateTime(Year, Month, 1).ToString("MMM yyyy");
        public string ShortMonthYear => new DateTime(Year, Month, 1).ToString("MM/yy");
    }

    // ============ Event Category ViewModel ============
    public class EventCategoryViewModel
    {
        [Display(Name = "Category")]
        public string CategoryName { get; set; } = string.Empty;

        [Display(Name = "Count")]
        public int Count { get; set; }

        // Helper properties for chart colors
        public string ChartColor => CategoryName switch
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

        public string CategoryIcon => CategoryName switch
        {
            "Adventure" => "bi-compass",
            "Cultural" => "bi-palette",
            "Food & Drink" => "bi-cup-straw",
            "Nature" => "bi-tree",
            "Urban" => "bi-building",
            "Relaxation" => "bi-sun",
            "Educational" => "bi-book",
            "Sports" => "bi-bicycle",
            _ => "bi-star"
        };
    }

    // ============ Statistics Card ViewModel ============
    public class StatisticsCardViewModel
    {
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Value")]
        public string Value { get; set; } = string.Empty;

        [Display(Name = "Growth")]
        public decimal? Growth { get; set; }

        [Display(Name = "Icon")]
        public string Icon { get; set; } = "bi-graph-up";

        [Display(Name = "Color")]
        public string Color { get; set; } = "primary";

        // Helper properties
        public string GrowthClass => Growth >= 0 ? "text-success" : "text-danger";
        public string GrowthIcon => Growth >= 0 ? "bi-arrow-up" : "bi-arrow-down";
        public string FormattedGrowth => Growth.HasValue
            ? $"{(Growth >= 0 ? "+" : "")}{Growth:F1}%"
            : "N/A";
    }
}
