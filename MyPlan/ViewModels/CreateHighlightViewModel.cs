using System.ComponentModel.DataAnnotations;

namespace MyPlan.ViewModels.HighlightVMs
{
    public class CreateHighlightViewModel
    {
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

        [Required(ErrorMessage = "Image is required")]
        [Display(Name = "Image")]
        public IFormFile Image { get; set; } = null!;

        [Required(ErrorMessage = "Date is required")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Date")]
        public DateTime HighlightTime { get; set; }
    }
}
