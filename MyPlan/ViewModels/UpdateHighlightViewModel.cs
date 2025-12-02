using System.ComponentModel.DataAnnotations;

namespace MyPlan.ViewModels.HighlightVMs
{
    public class UpdateHighlightViewModel
    {
        [Required]
        public int HighlightId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(255)]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required")]
        [Display(Name = "Content")]
        public string Content { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Image")]
        public IFormFile? Image { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Date")]
        public DateTime HighlightTime { get; set; }
    }
}
