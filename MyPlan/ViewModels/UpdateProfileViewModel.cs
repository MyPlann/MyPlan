using System.ComponentModel.DataAnnotations;

namespace MyPlan.ViewModels.ProfileVMs
{
    public class UpdateProfileViewModel
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(255)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(255)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email")]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(20)]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Bio")]
        public string? Bio { get; set; }

        [Display(Name = "Profile Picture")]
        public IFormFile? Avatar { get; set; }
    }
}
