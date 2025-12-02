using System.ComponentModel.DataAnnotations;

namespace MyPlan.ViewModels.ItineraryVMs
{
    public class CreateItineraryViewModel
    {
        [Required(ErrorMessage = "Please select an experience")]
        [Display(Name = "Experience")]
        public int ExperienceId { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateOnly StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateOnly EndDate { get; set; }

        [Required(ErrorMessage = "Day number is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Day must be at least 1")]
        [Display(Name = "Day")]
        public int Day { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500)]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;
    }
}
