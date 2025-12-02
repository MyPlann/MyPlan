using System.ComponentModel.DataAnnotations;

namespace MyPlan.ViewModels.ReviewVMs
{
    public class UpdateReviewViewModel
    {
        [Required]
        public int ReviewId { get; set; }

        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        [Display(Name = "Rating")]
        public int Rating { get; set; }

        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        [Display(Name = "Comment")]
        public string? Comment { get; set; }
    }
}
