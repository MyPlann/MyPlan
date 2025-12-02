using MyPlan.Shared;
using MyPlan.ViewModels.Validation;
using System.ComponentModel.DataAnnotations;

namespace MyPlan.ViewModels.AdminVMs.ProfileVMs
{
    // ============ Profile View ViewModel ============
    public class ProfileViewModel
    {
        // User Info
        public int UserId { get; set; }

        [Display(Name = "Full Name")]
        public string? FullName { get; set; }

        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Profile Image")]
        public string? Image { get; set; }

        // Admin Info
        public int AdminId { get; set; }

        [Display(Name = "First Name")]
        public string? AdminFirstName { get; set; }

        [Display(Name = "Last Name")]
        public string? AdminLastName { get; set; }

        [Display(Name = "Phone Number")]
        public string? AdminPhoneNumber { get; set; }

        [Display(Name = "Position")]
        public string? AdminPosition { get; set; }

        [Display(Name = "Member Since")]
        public DateTime AddedAt { get; set; }

        public string ActiveTab { get; set; } = "profile";

        // Helper properties
        public bool HasImage => !string.IsNullOrEmpty(Image);
        public string Initials => GetInitials(FullName);
        public string MemberSinceDays => $"{(DateTime.Now - AddedAt).Days} days";

        private string GetInitials(string? name)
        {
            if (string.IsNullOrEmpty(name))
                return "A";

            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return "A";

            if (parts.Length == 1)
                return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpper();

            return $"{parts[0][0]}{parts[parts.Length - 1][0]}".ToUpper();
        }

        public Dictionary<string, string> PositionOptions => StatusHelper.GetAdminPositions();
    }

    // ============ Update Profile ViewModel ============
    public class UpdateProfileViewModel
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 100 characters")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters")]
        [Display(Name = "First Name")]
        public string AdminFirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters")]
        [Display(Name = "Last Name")]
        public string AdminLastName { get; set; } = string.Empty;

        [PhoneNumber]
        [Display(Name = "Phone Number")]
        public string? AdminPhoneNumber { get; set; }

        [Required(ErrorMessage = "Position is required")]
        [StringLength(50, ErrorMessage = "Position cannot exceed 50 characters")]
        [Display(Name = "Position")]
        public string AdminPosition { get; set; } = string.Empty;

        [Display(Name = "Profile Image")]
        public IFormFile? ImageFile { get; set; }

        public string? CurrentImage { get; set; }

        public string Tab { get; set; } = "profile";

        // Available options
        public Dictionary<string, string> PositionOptions => StatusHelper.GetAdminPositions();
    }

    // ============ Update Password ViewModel ============
    public class UpdatePasswordViewModel
    {
        [Required(ErrorMessage = "Current password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number and one special character")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm your password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    // ============ Profile Statistics ViewModel ============
    public class ProfileStatisticsViewModel
    {
        public int UserId { get; set; }

        [Display(Name = "Total Experiences Created")]
        public int TotalExperiences { get; set; }

        [Display(Name = "Total Highlights Posted")]
        public int TotalHighlights { get; set; }

        [Display(Name = "Total Bookings Managed")]
        public int TotalBookings { get; set; }

        [Display(Name = "Total Revenue Generated")]
        [DataType(DataType.Currency)]
        public decimal TotalRevenue { get; set; }

        [Display(Name = "Member Since")]
        public DateTime MemberSince { get; set; }

        [Display(Name = "Last Login")]
        public DateTime? LastLogin { get; set; }

        // Helper properties
        public int DaysSinceMember => (DateTime.Now - MemberSince).Days;
        public string LastLoginFormatted => LastLogin.HasValue
            ? LastLogin.Value.ToString("MMM dd, yyyy 'at' h:mm tt")
            : "Never";
    }

    // ============ Activity Log ViewModel ============
    public class ActivityLogViewModel
    {
        public int ActivityId { get; set; }

        [Display(Name = "Activity Type")]
        public string? ActivityType { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Date & Time")]
        public DateTime ActivityTime { get; set; }

        [Display(Name = "IP Address")]
        public string? IpAddress { get; set; }

        // Helper properties
        public string ActivityIcon => ActivityType switch
        {
            "Login" => "bi-box-arrow-in-right",
            "Logout" => "bi-box-arrow-left",
            "Create" => "bi-plus-circle",
            "Update" => "bi-pencil-square",
            "Delete" => "bi-trash",
            "View" => "bi-eye",
            _ => "bi-circle"
        };

        public string ActivityBadgeClass => ActivityType switch
        {
            "Login" => "success",
            "Logout" => "secondary",
            "Create" => "primary",
            "Update" => "info",
            "Delete" => "danger",
            "View" => "light",
            _ => "secondary"
        };

        public string TimeAgo
        {
            get
            {
                var timeSpan = DateTime.Now - ActivityTime;

                if (timeSpan.TotalMinutes < 1)
                    return "Just now";
                if (timeSpan.TotalMinutes < 60)
                    return $"{(int)timeSpan.TotalMinutes} min ago";
                if (timeSpan.TotalHours < 24)
                    return $"{(int)timeSpan.TotalHours} hour{((int)timeSpan.TotalHours > 1 ? "s" : "")} ago";
                if (timeSpan.TotalDays < 7)
                    return $"{(int)timeSpan.TotalDays} day{((int)timeSpan.TotalDays > 1 ? "s" : "")} ago";

                return ActivityTime.ToString("MMM dd, yyyy");
            }
        }
    }

    // ============ Notification Settings ViewModel ============
    public class NotificationSettingsViewModel
    {
        [Display(Name = "Email Notifications")]
        public bool EmailNotifications { get; set; }

        [Display(Name = "New Booking Notifications")]
        public bool NewBookingNotifications { get; set; }

        [Display(Name = "Payment Notifications")]
        public bool PaymentNotifications { get; set; }

        [Display(Name = "Review Notifications")]
        public bool ReviewNotifications { get; set; }

        [Display(Name = "System Updates")]
        public bool SystemUpdates { get; set; }

        [Display(Name = "Marketing Emails")]
        public bool MarketingEmails { get; set; }
    }

    // ============ Security Settings ViewModel ============
    public class SecuritySettingsViewModel
    {
        [Display(Name = "Two-Factor Authentication")]
        public bool TwoFactorEnabled { get; set; }

        [Display(Name = "Login Alerts")]
        public bool LoginAlerts { get; set; }

        [Display(Name = "Session Timeout (minutes)")]
        [Range(5, 240, ErrorMessage = "Session timeout must be between 5 and 240 minutes")]
        public int SessionTimeout { get; set; } = 120;

        [Display(Name = "Last Password Change")]
        public DateTime? LastPasswordChange { get; set; }

        // Helper properties
        public string LastPasswordChangeFormatted => LastPasswordChange.HasValue
            ? LastPasswordChange.Value.ToString("MMM dd, yyyy")
            : "Never";

        public int DaysSincePasswordChange => LastPasswordChange.HasValue
            ? (DateTime.Now - LastPasswordChange.Value).Days
            : 0;

        public bool PasswordChangeRecommended => DaysSincePasswordChange > 90;
    }
}
