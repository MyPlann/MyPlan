using MyPlan.Shared;
using System.ComponentModel.DataAnnotations;

namespace MyPlan.ViewModels.AdminVMs.ExperienceVMs
{
    // ============ List ViewModel ============
    public class ExperienceListViewModel
    {
        public int ExperienceId { get; set; }

        [Display(Name = "Title")]
        public string? ExperienceTitle { get; set; }

        [Display(Name = "Type")]
        public string? ExperienceType { get; set; }

        [Display(Name = "Location")]
        public string? ExperienceLocation { get; set; }

        [Display(Name = "Min Price")]
        [DataType(DataType.Currency)]
        public decimal ExperienceMinPrice { get; set; }

        [Display(Name = "Max Price")]
        [DataType(DataType.Currency)]
        public decimal ExperienceMaxPrice { get; set; }

        [Display(Name = "Start Date")]
        public DateOnly? ExperienceStartDate { get; set; }

        [Display(Name = "End Date")]
        public DateOnly? ExperienceEndDate { get; set; }

        [Display(Name = "Max Capacity")]
        public int MaxCapacity { get; set; }

        [Display(Name = "Added At")]
        public DateTime AddedAt { get; set; }

        public int ImageCount { get; set; }
        public int DetailCount { get; set; }
        public string? FirstImage { get; set; }

        // Helper properties
        public string PriceRange => $"{ExperienceMinPrice:C} - {ExperienceMaxPrice:C}";
        public bool HasImage => !string.IsNullOrEmpty(FirstImage);

        public string TypeBadgeClass => ExperienceType switch
        {
            "Adventure" => "danger",
            "Cultural" => "primary",
            "Food & Drink" => "warning",
            "Nature" => "success",
            "Urban" => "info",
            "Relaxation" => "secondary",
            "Educational" => "dark",
            "Sports" => "light",
            _ => "secondary"
        };
    }

    // ============ Form ViewModel ============
    public class ExperienceFormViewModel
    {
        public int? ExperienceId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        [Display(Name = "Experience Title")]
        public string ExperienceTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [Display(Name = "Description")]
        public string ExperienceDescription { get; set; } = string.Empty;

        [Required(ErrorMessage = "Type is required")]
        [StringLength(50, ErrorMessage = "Type cannot exceed 50 characters")]
        [Display(Name = "Experience Type")]
        public string ExperienceType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required")]
        [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
        [Display(Name = "Location")]
        public string ExperienceLocation { get; set; } = string.Empty;

        [Required(ErrorMessage = "Minimum price is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Minimum price must be greater than or equal to 0")]
        [Display(Name = "Minimum Price")]
        public decimal ExperienceMinPrice { get; set; }

        [Required(ErrorMessage = "Maximum price is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Maximum price must be greater than or equal to 0")]
        [Display(Name = "Maximum Price")]
        public decimal ExperienceMaxPrice { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        [Display(Name = "Start Date")]
        public DateOnly? ExperienceStartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        [Display(Name = "End Date")]
        public DateOnly? ExperienceEndDate { get; set; }

        [Required(ErrorMessage = "Maximum capacity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Maximum capacity must be at least 1")]
        [Display(Name = "Maximum Capacity")]
        public int MaxCapacity { get; set; }

        [Required(ErrorMessage = "Latitude is required")]
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        [Display(Name = "Latitude")]
        public decimal Lat { get; set; }

        [Required(ErrorMessage = "Longitude is required")]
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        [Display(Name = "Longitude")]
        public decimal Long { get; set; }

        [Display(Name = "Images")]
        public List<IFormFile>? Images { get; set; }

        public List<ExistingImageViewModel>? ExistingImages { get; set; }

        [Display(Name = "Experience Details")]
        public List<ExperienceDetailViewModel>? Details { get; set; }

        public bool IsEdit => ExperienceId.HasValue;

        // Available options
        public Dictionary<string, string> ExperienceTypes => StatusHelper.GetExperienceTypes();

        // Custom validation
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ExperienceMaxPrice < ExperienceMinPrice)
            {
                yield return new ValidationResult(
                    "Maximum price must be greater than or equal to minimum price",
                    new[] { nameof(ExperienceMaxPrice) });
            }

            if (ExperienceEndDate < ExperienceStartDate)
            {
                yield return new ValidationResult(
                    "End date must be after or equal to start date",
                    new[] { nameof(ExperienceEndDate) });
            }

            if (Details == null || !Details.Any())
            {
                yield return new ValidationResult(
                    "At least one experience detail is required",
                    new[] { nameof(Details) });
            }
        }
    }

    // ============ Experience Detail ViewModel ============
    public class ExperienceDetailViewModel
    {
        public int? ExperienceDetailId { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [Display(Name = "Date")]
        public DateOnly? Date { get; set; }

        [Required(ErrorMessage = "Time is required")]
        [Display(Name = "Time")]
        public TimeOnly? Time { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0")]
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        public bool IsNew => !ExperienceDetailId.HasValue || ExperienceDetailId == 0;
    }

    // ============ Existing Image ViewModel ============
    public class ExistingImageViewModel
    {
        public int ImageId { get; set; }
        public string? ImageAttachment { get; set; }
    }

    // ============ Details View ViewModel ============
    public class ExperienceDetailsViewModel
    {
        public int ExperienceId { get; set; }

        [Display(Name = "Title")]
        public string? ExperienceTitle { get; set; }

        [Display(Name = "Description")]
        public string? ExperienceDescription { get; set; }

        [Display(Name = "Type")]
        public string? ExperienceType { get; set; }

        [Display(Name = "Location")]
        public string? ExperienceLocation { get; set; }

        [Display(Name = "Min Price")]
        [DataType(DataType.Currency)]
        public decimal ExperienceMinPrice { get; set; }

        [Display(Name = "Max Price")]
        [DataType(DataType.Currency)]
        public decimal ExperienceMaxPrice { get; set; }

        [Display(Name = "Start Date")]
        public DateOnly? ExperienceStartDate { get; set; }

        [Display(Name = "End Date")]
        public DateOnly? ExperienceEndDate { get; set; }

        [Display(Name = "Max Capacity")]
        public int MaxCapacity { get; set; }

        [Display(Name = "Latitude")]
        public decimal Lat { get; set; }

        [Display(Name = "Longitude")]
        public decimal Long { get; set; }

        [Display(Name = "Added At")]
        public DateTime AddedAt { get; set; }

        public List<ImageViewModel> Images { get; set; } = new();
        public List<ExperienceDetailInfoViewModel> Details { get; set; } = new();

        // Statistics
        public int TotalBookings { get; set; }
        public int TotalReviews { get; set; }
        public double AverageRating { get; set; }

        // Helper properties
        public string PriceRange => $"{ExperienceMinPrice:C} - {ExperienceMaxPrice:C}";
        public string RatingStars => StatusHelper.GetRatingStars((int)Math.Round(AverageRating));
    }

    // ============ Image ViewModel ============
    public class ImageViewModel
    {
        public int ImageId { get; set; }
        public string? ImageAttachment { get; set; }
        public DateTime ImageTime { get; set; }
    }

    // ============ Experience Detail Info ViewModel ============
    public class ExperienceDetailInfoViewModel
    {
        public int ExperienceDetailId { get; set; }

        [Display(Name = "Date")]
        public DateOnly? Date { get; set; }

        [Display(Name = "Time")]
        public TimeOnly? Time { get; set; }

        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Display(Name = "Status")]
        public string? Status { get; set; }

        public string StatusBadgeClass => Status switch
        {
            StatusHelper.ExperienceActive => "success",
            StatusHelper.ExperienceInactive => "secondary",
            StatusHelper.ExperienceSoldOut => "warning",
            StatusHelper.ExperienceCancelled => "danger",
            _ => "secondary"
        };
    }

    // ============ Filter ViewModel ============
    public class ExperienceFilterViewModel
    {
        [Display(Name = "Search")]
        public string? SearchTerm { get; set; }

        [Display(Name = "Type")]
        public string? ExperienceType { get; set; }

        [Display(Name = "Location")]
        public string? Location { get; set; }

        [Display(Name = "Min Price")]
        [Range(0, double.MaxValue)]
        public decimal? MinPrice { get; set; }

        [Display(Name = "Max Price")]
        [Range(0, double.MaxValue)]
        public decimal? MaxPrice { get; set; }

        [Display(Name = "From Date")]
        [DataType(DataType.Date)]
        public DateOnly? FromDate { get; set; }

        [Display(Name = "To Date")]
        [DataType(DataType.Date)]
        public DateOnly? ToDate { get; set; }

        [Display(Name = "Sort By")]
        public string SortBy { get; set; } = "date_desc";

        public bool HasFilters =>
            !string.IsNullOrEmpty(SearchTerm)
            || !string.IsNullOrEmpty(ExperienceType)
            || !string.IsNullOrEmpty(Location)
            || MinPrice.HasValue
            || MaxPrice.HasValue
            || FromDate.HasValue
            || ToDate.HasValue;

        // Available options
        public Dictionary<string, string> ExperienceTypes => StatusHelper.GetExperienceTypes();

        public Dictionary<string, string> SortOptions => new()
        {
            { "date_desc", "Newest First" },
            { "date_asc", "Oldest First" },
            { "price_asc", "Price: Low to High" },
            { "price_desc", "Price: High to Low" },
            { "title_asc", "Title: A-Z" },
            { "title_desc", "Title: Z-A" }
        };
    }
}
