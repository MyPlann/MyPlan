using System.ComponentModel.DataAnnotations;

namespace MyPlan.ViewModels.AdminVMs.HighlightVMs
{
    // ============ List ViewModel ============
    public class HighlightListViewModel
    {
        public int HighlightId { get; set; }

        [Display(Name = "Title")]
        public string? HighlightTitle { get; set; }

        [Display(Name = "Description")]
        public string? HighlightDescription { get; set; }

        [Display(Name = "Image")]
        public string? HighlightImage { get; set; }

        [Display(Name = "Highlight Time")]
        public DateTime HighlightTime { get; set; }

        [Display(Name = "Added At")]
        public DateTime AddedAt { get; set; }

        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }

        [Display(Name = "Type")]
        public string? CreatedByType { get; set; }

        [Display(Name = "Email")]
        public string? CreatedByEmail { get; set; }

        // Helper properties
        public string CreatedByBadgeClass => CreatedByType switch
        {
            "Admin" => "primary",
            "Visitor" => "success",
            _ => "secondary"
        };

        public string CreatedByIcon => CreatedByType switch
        {
            "Admin" => "bi-shield-check",
            "Visitor" => "bi-person",
            _ => "bi-question-circle"
        };

        public string TruncatedDescription
        {
            get
            {
                if (string.IsNullOrEmpty(HighlightDescription))
                    return string.Empty;

                return HighlightDescription.Length > 100
                    ? HighlightDescription.Substring(0, 100) + "..."
                    : HighlightDescription;
            }
        }

        public string TimeAgo
        {
            get
            {
                var timeSpan = DateTime.Now - HighlightTime;

                if (timeSpan.TotalMinutes < 1)
                    return "Just now";
                if (timeSpan.TotalMinutes < 60)
                    return $"{(int)timeSpan.TotalMinutes} min ago";
                if (timeSpan.TotalHours < 24)
                    return $"{(int)timeSpan.TotalHours} hour{((int)timeSpan.TotalHours > 1 ? "s" : "")} ago";
                if (timeSpan.TotalDays < 7)
                    return $"{(int)timeSpan.TotalDays} day{((int)timeSpan.TotalDays > 1 ? "s" : "")} ago";

                return HighlightTime.ToString("MMM dd, yyyy");
            }
        }
    }

    // ============ Details ViewModel ============
    public class HighlightDetailsViewModel
    {
        public int HighlightId { get; set; }

        [Display(Name = "Title")]
        public string? HighlightTitle { get; set; }

        [Display(Name = "Content")]
        public string? HighlightContent { get; set; }

        [Display(Name = "Description")]
        public string? HighlightDescription { get; set; }

        [Display(Name = "Image")]
        public string? HighlightImage { get; set; }

        [Display(Name = "Highlight Time")]
        public DateTime HighlightTime { get; set; }

        [Display(Name = "Added At")]
        public DateTime AddedAt { get; set; }

        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }

        [Display(Name = "Type")]
        public string? CreatedByType { get; set; }

        [Display(Name = "Email")]
        public string? CreatedByEmail { get; set; }

        [Display(Name = "Profile Image")]
        public string? CreatedByImage { get; set; }

        // Helper properties
        public string CreatedByBadgeClass => CreatedByType switch
        {
            "Admin" => "primary",
            "Visitor" => "success",
            _ => "secondary"
        };

        public string CreatedByIcon => CreatedByType switch
        {
            "Admin" => "bi-shield-check",
            "Visitor" => "bi-person",
            _ => "bi-question-circle"
        };

        public bool HasImage => !string.IsNullOrEmpty(HighlightImage);
        public bool HasContent => !string.IsNullOrEmpty(HighlightContent);
    }

    // ============ Create/Edit ViewModel ============
    public class HighlightFormViewModel
    {
        public int? HighlightId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        [Display(Name = "Title")]
        public string HighlightTitle { get; set; } = string.Empty;

        [Display(Name = "Content")]
        public string? HighlightContent { get; set; }

        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters")]
        [Display(Name = "Description")]
        public string? HighlightDescription { get; set; }

        [Display(Name = "Image")]
        public IFormFile? ImageFile { get; set; }

        [Display(Name = "Current Image")]
        public string? CurrentImage { get; set; }

        [Display(Name = "Remove Current Image")]
        public bool RemoveCurrentImage { get; set; }

        [Display(Name = "Highlight Time")]
        public DateTime? HighlightTime { get; set; }

        // For admin creation
        public int? AdminId { get; set; }

        // For visitor creation
        public int? VisitorId { get; set; }

        public bool IsEdit => HighlightId.HasValue;
    }

    // ============ Bulk Delete ViewModel ============
    public class BulkDeleteHighlightsViewModel
    {
        [Required(ErrorMessage = "Please select at least one highlight")]
        [MinLength(1, ErrorMessage = "Please select at least one highlight")]
        public int[] HighlightIds { get; set; } = Array.Empty<int>();
    }

    // ============ Filter ViewModel ============
    public class HighlightFilterViewModel
    {
        [Display(Name = "Search")]
        public string? SearchTerm { get; set; }

        [Display(Name = "Created By Type")]
        public string? CreatedByType { get; set; }

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
            || !string.IsNullOrEmpty(CreatedByType)
            || FromDate.HasValue
            || ToDate.HasValue;
    }
}
