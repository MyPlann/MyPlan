using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyPlan.Data;
using MyPlan.Shared;
using MyPlan.ViewModels.AdminVMs.ProfileVMs;
using System.Security.Claims;

namespace MyPlan.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    [Route("Admin/Profile")]
    public class ProfileController : Controller
    {
        private readonly MyTripDbContext _context;
        private readonly ILogger<ProfileController> _logger;
        private readonly IFileUploadService _fileUploadService;

        public ProfileController(
            MyTripDbContext context,
            ILogger<ProfileController> logger,
            IFileUploadService fileUploadService)
        {
            _context = context;
            _logger = logger;
            _fileUploadService = fileUploadService;
        }

        /// <summary>
        /// Display the admin profile
        /// </summary>
        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(string? tab)
        {
            try
            {
                var userId = User.GetUserId();
                if (userId == null)
                {
                    return RedirectToAction("Login", "Auth");
                }

                var user = await _context.Users
                    .Include(u => u.Admins)
                    .FirstOrDefaultAsync(u => u.UserId == userId.Value);

                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("Login", "Auth");
                }

                var admin = user.Admins.FirstOrDefault();
                if (admin == null)
                {
                    TempData["Error"] = "Admin profile not found.";
                    return RedirectToAction("Login", "Auth");
                }

                var viewModel = new ProfileViewModel
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    Image = user.Image,
                    AdminId = admin.AdminId,
                    AdminFirstName = admin.AdminFirstName,
                    AdminLastName = admin.AdminLastName,
                    AdminPhoneNumber = admin.AdminPhoneNumber,
                    AdminPosition = admin.AdminPosition,
                    AddedAt = user.AddedAt ?? DateTime.Now,
                    ActiveTab = tab ?? "profile"
                };

                return View("~/Views/Admin/Profile/Index.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading profile");
                TempData["Error"] = "Failed to load profile. Please try again.";
                return RedirectToAction("Index", "Dashboard");
            }
        }

        /// <summary>
        /// Update the admin profile
        /// </summary>
        [HttpPost("Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index), new { tab = model.Tab })
                    .WithErrors(ModelState);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var userId = User.GetUserId();
                if (userId == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("Login", "Auth");
                }

                var user = await _context.Users
                    .Include(u => u.Admins)
                    .FirstOrDefaultAsync(u => u.UserId == userId.Value);

                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("Login", "Auth");
                }

                var admin = user.Admins.FirstOrDefault();
                if (admin == null)
                {
                    TempData["Error"] = "Admin profile not found.";
                    return RedirectToAction("Login", "Auth");
                }

                // Check if email is already taken by another user
                var emailExists = await _context.Users
                    .AnyAsync(u => u.Email == model.Email && u.UserId != userId.Value);

                if (emailExists)
                {
                    ModelState.AddModelError("Email", "Email is already taken.");
                    TempData["Error"] = "Email is already taken.";
                    return RedirectToAction(nameof(Index), new { tab = model.Tab });
                }

                // Update user data
                user.FullName = model.FullName;
                user.Email = model.Email;

                // Update admin data
                admin.AdminFirstName = model.AdminFirstName;
                admin.AdminLastName = model.AdminLastName;
                admin.AdminPhoneNumber = model.AdminPhoneNumber;
                admin.AdminPosition = model.AdminPosition;

                // Handle image upload
                if (model.ImageFile != null)
                {
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(user.Image))
                    {
                        try
                        {
                            await _fileUploadService.DeleteFileAsync(user.Image);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to delete old profile image");
                        }
                    }

                    // Upload new image
                    var path = await _fileUploadService.UploadFileAsync(model.ImageFile, "profiles");
                    user.Image = path;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = "Profile updated successfully!";
                return RedirectToAction(nameof(Index), new { tab = model.Tab });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating profile");
                TempData["Error"] = "Failed to update profile. Please try again.";
                return RedirectToAction(nameof(Index), new { tab = model.Tab });
            }
        }

        /// <summary>
        /// Update password
        /// </summary>
        [HttpPost("UpdatePassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please correct the errors and try again.";
                return RedirectToAction(nameof(Index), new { tab = "security" });
            }

            try
            {
                var userId = User.GetUserId();
                if (userId == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("Login", "Auth");
                }

                var user = await _context.Users.FindAsync(userId.Value);
                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("Login", "Auth");
                }

                // Verify current password
                if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.PasswordHash))
                {
                    TempData["Error"] = "Current password is incorrect.";
                    return RedirectToAction(nameof(Index), new { tab = "security" });
                }

                // Update password
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Password updated successfully!";
                return RedirectToAction(nameof(Index), new { tab = "security" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating password");
                TempData["Error"] = "Failed to update password. Please try again.";
                return RedirectToAction(nameof(Index), new { tab = "security" });
            }
        }

        /// <summary>
        /// Delete profile image
        /// </summary>
        [HttpPost("DeleteImage")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage()
        {
            try
            {
                var userId = User.GetUserId();
                if (userId == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                var user = await _context.Users.FindAsync(userId.Value);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                if (!string.IsNullOrEmpty(user.Image))
                {
                    try
                    {
                        await _fileUploadService.DeleteFileAsync(user.Image);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to delete profile image file");
                    }

                    user.Image = null;
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = "Profile image deleted successfully!";
                return RedirectToAction(nameof(Index), new { tab = "profile" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting profile image");
                TempData["Error"] = "Failed to delete image. Please try again.";
                return RedirectToAction(nameof(Index), new { tab = "profile" });
            }
        }
    }
}