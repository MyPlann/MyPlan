using System.ComponentModel.DataAnnotations;

namespace MyPlan.ViewModels.FriendVMs
{
    public class InviteFriendViewModel
    {
        [Required(ErrorMessage = "Please select an experience")]
        public int ExperienceId { get; set; }

        [Required(ErrorMessage = "Please select a date and time")]
        public int ExperienceDetailId { get; set; }

        [StringLength(500)]
        [Display(Name = "Personal Message")]
        public string? Message { get; set; }

        public int FriendId { get; set; }
    }
}
