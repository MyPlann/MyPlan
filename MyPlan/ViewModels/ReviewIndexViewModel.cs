using MyPlan.Shared;
using System.ComponentModel.DataAnnotations;

namespace MyPlan.ViewModels.AdminVMs.ReviewVMs
{
    // ============ Index ViewModel ============
    public class ReviewIndexViewModel
    {
        public List<ReviewListViewModel> Reviews { get; set; } = new();
        public ReviewFilterViewModel Filter { get; set; } = new();

        // Statistics
        [Display(Name = "Total Reviews")]
        public int TotalReviews { get; set; }

        [Display(Name = "Average Rating")]
        public double AverageRating { get; set; }

        public int FiveStarCount { get; set; }
        public int FourStarCount { get; set; }
        public int ThreeStarCount { get; set; }
        public int TwoStarCount { get; set; }
        public int OneStarCount { get; set; }

        // Helper properties
        public string AverageRatingFormatted => AverageRating.ToString("F1");
        public string AverageRatingStars => StatusHelper.GetRatingStars((int)Math.Round(AverageRating));

        public int FiveStarPercentage => TotalReviews > 0 ? (FiveStarCount * 100) / TotalReviews : 0;
        public int FourStarPercentage => TotalReviews > 0 ? (FourStarCount * 100) / TotalReviews : 0;
        public int ThreeStarPercentage => TotalReviews > 0 ? (ThreeStarCount * 100) / TotalReviews : 0;
        public int TwoStarPercentage => TotalReviews > 0 ? (TwoStarCount * 100) / TotalReviews : 0;
        public int OneStarPercentage => TotalReviews > 0 ? (OneStarCount * 100) / TotalReviews : 0;
    }

    // ============ List ViewModel ============
    public class ReviewListViewModel
    {
        public int ReviewId { get; set; }

        [Display(Name = "Rating")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int ReviewRating { get; set; }

        [Display(Name = "Comment")]
        public string? ReviewComment { get; set; }

        [Display(Name = "Review Time")]
        public DateTime ReviewTime { get; set; }

        [Display(Name = "Added At")]
        public DateTime AddedAt { get; set; }

        // Visitor Info
        [Display(Name = "Visitor")]
        public string? VisitorName { get; set; }

        [Display(Name = "Email")]
        public string? VisitorEmail { get; set; }

        [Display(Name = "Image")]
        public string? VisitorImage { get; set; }

        // Experience Info
        public int ExperienceId { get; set; }

        [Display(Name = "Experience")]
        public string? ExperienceTitle { get; set; }

        [Display(Name = "Type")]
        public string? ExperienceType { get; set; }

        [Display(Name = "Location")]
        public string? ExperienceLocation { get; set; }

        // Booking Info
        public int? BookingId { get; set; }

        // Helper properties
        public string RatingStars => StatusHelper.GetRatingStars(ReviewRating);

        public string RatingBadgeClass => ReviewRating switch
        {
            5 => "success",
            4 => "info",
            3 => "warning",
            2 => "orange",
            1 => "danger",
            _ => "secondary"
        };

        public string TruncatedComment
        {
            get
            {
                if (string.IsNullOrEmpty(ReviewComment))
                    return string.Empty;

                return ReviewComment.Length > 100
                    ? ReviewComment.Substring(0, 100) + "..."
                    : ReviewComment;
            }
        }

        public string TimeAgo
        {
            get
            {
                var timeSpan = DateTime.Now - ReviewTime;

                if (timeSpan.TotalMinutes < 1)
                    return "Just now";
                if (timeSpan.TotalMinutes < 60)
                    return $"{(int)timeSpan.TotalMinutes} min ago";
                if (timeSpan.TotalHours < 24)
                    return $"{(int)timeSpan.TotalHours} hour{((int)timeSpan.TotalHours > 1 ? "s" : "")} ago";
                if (timeSpan.TotalDays < 7)
                    return $"{(int)timeSpan.TotalDays} day{((int)timeSpan.TotalDays > 1 ? "s" : "")} ago";
                if (timeSpan.TotalDays < 30)
                    return $"{(int)(timeSpan.TotalDays / 7)} week{((int)(timeSpan.TotalDays / 7) > 1 ? "s" : "")} ago";

                return ReviewTime.ToString("MMM dd, yyyy");
            }
        }
    }

    // ============ Details ViewModel ============
    public class ReviewDetailsViewModel
    {
        public int ReviewId { get; set; }

        [Display(Name = "Rating")]
        public int ReviewRating { get; set; }

        [Display(Name = "Comment")]
        public string? ReviewComment { get; set; }

        [Display(Name = "Review Time")]
        public DateTime ReviewTime { get; set; }

        [Display(Name = "Added At")]
        public DateTime AddedAt { get; set; }

        // Visitor Info
        [Display(Name = "Visitor Name")]
        public string? VisitorName { get; set; }

        [Display(Name = "Email")]
        public string? VisitorEmail { get; set; }

        [Display(Name = "Phone")]
        public string? VisitorPhone { get; set; }

        [Display(Name = "Image")]
        public string? VisitorImage { get; set; }

        // Experience Info
        public int ExperienceId { get; set; }

        [Display(Name = "Experience")]
        public string? ExperienceTitle { get; set; }

        [Display(Name = "Type")]
        public string? ExperienceType { get; set; }

        [Display(Name = "Location")]
        public string? ExperienceLocation { get; set; }

        [Display(Name = "Description")]
        public string? ExperienceDescription { get; set; }

        // Booking Info
        public int? BookingId { get; set; }

        [Display(Name = "Booking Date")]
        public DateOnly? BookingDate { get; set; }

        [Display(Name = "Booking Status")]
        public string? BookingStatus { get; set; }

        // Helper properties
        public string RatingStars => StatusHelper.GetRatingStars(ReviewRating);
        public string RatingText => StatusHelper.GetReviewRatings()[ReviewRating];
        public bool HasBooking => BookingId.HasValue;

        public string RatingBadgeClass => ReviewRating switch
        {
            5 => "success",
            4 => "info",
            3 => "warning",
            2 => "orange",
            1 => "danger",
            _ => "secondary"
        };
    }

    // ============ Filter ViewModel ============
    public class ReviewFilterViewModel
    {
        [Display(Name = "Search")]
        public string? SearchTerm { get; set; }

        [Display(Name = "Rating")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int? Rating { get; set; }

        [Display(Name = "Experience Type")]
        public string? ExperienceType { get; set; }

        [Display(Name = "From Date")]
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [Display(Name = "To Date")]
        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        [Display(Name = "Sort By")]
        public string SortBy { get; set; } = "date_desc";

        public bool HasFilters =>
            !string.IsNullOrEmpty(SearchTerm)
            || Rating.HasValue
            || !string.IsNullOrEmpty(ExperienceType)
            || FromDate.HasValue
            || ToDate.HasValue;

        // Available options for dropdowns
        public Dictionary<string, string> SortOptions => new()
        {
            { "date_desc", "Newest First" },
            { "date_asc", "Oldest First" },
            { "rating_desc", "Highest Rating" },
            { "rating_asc", "Lowest Rating" }
        };

        public Dictionary<int, string> RatingOptions => StatusHelper.GetReviewRatings();
        public Dictionary<string, string> ExperienceTypeOptions => StatusHelper.GetExperienceTypes();
    }

    // ============ Create/Edit ViewModel ============
    public class ReviewFormViewModel
    {
        public int? ReviewId { get; set; }

        [Required(ErrorMessage = "Experience is required")]
        [Display(Name = "Experience")]
        public int ExperienceId { get; set; }

        [Required(ErrorMessage = "Visitor is required")]
        [Display(Name = "Visitor")]
        public int VisitorId { get; set; }

        [Display(Name = "Booking")]
        public int? BookingId { get; set; }

        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        [Display(Name = "Rating")]
        public int ReviewRating { get; set; }

        [Required(ErrorMessage = "Comment is required")]
        [StringLength(255, MinimumLength = 10, ErrorMessage = "Comment must be between 10 and 255 characters")]
        [Display(Name = "Comment")]
        public string ReviewComment { get; set; } = string.Empty;

        [Display(Name = "Review Time")]
        public DateTime? ReviewTime { get; set; }

        public bool IsEdit => ReviewId.HasValue;
    }

    // ============ Bulk Delete ViewModel ============
    public class BulkDeleteReviewsViewModel
    {
        [Required(ErrorMessage = "Please select at least one review")]
        [MinLength(1, ErrorMessage = "Please select at least one review")]
        public int[] ReviewIds { get; set; } = Array.Empty<int>();
    }

    // ============ Statistics ViewModel ============
    public class ReviewStatisticsViewModel
    {
        public int ExperienceId { get; set; }
        public string? ExperienceTitle { get; set; }

        [Display(Name = "Total Reviews")]
        public int TotalReviews { get; set; }

        [Display(Name = "Average Rating")]
        public double AverageRating { get; set; }

        public int FiveStarCount { get; set; }
        public int FourStarCount { get; set; }
        public int ThreeStarCount { get; set; }
        public int TwoStarCount { get; set; }
        public int OneStarCount { get; set; }

        public List<ReviewListViewModel> RecentReviews { get; set; } = new();

        // Helper properties
        public string AverageRatingFormatted => AverageRating.ToString("F1");
        public string AverageRatingStars => StatusHelper.GetRatingStars((int)Math.Round(AverageRating));
    }
}
