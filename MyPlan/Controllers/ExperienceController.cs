using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyPlan.Data;
using MyPlan.Models;
using MyPlan.ViewModels.AdminVMs.ExperienceVMs;
using MyPlan.Shared;

namespace MyPlan.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    [Route("Admin/Experience")]
    public class ExperienceController : Controller
    {
        private readonly MyTripDbContext _context;
        private readonly ILogger<ExperienceController> _logger;
        private readonly IFileUploadService _fileUploadService;

        public ExperienceController(
            MyTripDbContext context,
            ILogger<ExperienceController> logger,
            IFileUploadService fileUploadService)
        {
            _context = context;
            _logger = logger;
            _fileUploadService = fileUploadService;
        }

        /// <summary>
        /// Display a listing of experiences
        /// </summary>
        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var experiences = await _context.Experiences
                    .Include(e => e.Images)
                    .Include(e => e.ExperienceDetails)
                    .OrderByDescending(e => e.AddedAt)
                    .Select(e => new ExperienceListViewModel
                    {
                        ExperienceId = e.ExperienceId,
                        ExperienceTitle = e.ExperienceTitle,
                        ExperienceType = e.ExperienceType,
                        ExperienceLocation = e.ExperienceLocation,
                        ExperienceMinPrice = e.ExperienceMinPrice ?? 0,
                        ExperienceMaxPrice = e.ExperienceMaxPrice ?? 0,
                        ExperienceStartDate = e.ExperienceStartDate,
                        ExperienceEndDate = e.ExperienceEndDate,
                        MaxCapacity = e.MaxCapacity ?? 0,
                        AddedAt = e.AddedAt ?? DateTime.Now,
                        ImageCount = e.Images.Count,
                        DetailCount = e.ExperienceDetails.Count,
                        FirstImage = e.Images.OrderBy(i => i.ImageId).Select(i => i.ImageAttachment).FirstOrDefault()
                    })
                    .ToListAsync();
                return View("~/Views/Admin/Experience/Index.cshtml", experiences);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving experiences list");
                TempData["Error"] = "Failed to load experiences. Please try again.";
                return View(new List<ExperienceListViewModel>());
            }
        }

        /// <summary>
        /// Show the form for creating a new experience
        /// </summary>
        [HttpGet("Create")]
        public IActionResult Create()
        {
            var model = new ExperienceFormViewModel
            {
                ExperienceStartDate = DateOnly.FromDateTime(DateTime.Now),
                ExperienceEndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
            };

            //return View("CreateEdit", model);
            return View("~/Views/Admin/Experience/CreateEdit.cshtml", model);
        }

        /// <summary>
        /// Store a newly created experience
        /// </summary>
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExperienceFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                //return View("CreateEdit", model);
                return View("~/Views/Admin/Experience/CreateEdit.cshtml", model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Create experience
                var experience = new Experience
                {
                    ExperienceTitle = model.ExperienceTitle,
                    ExperienceDescription = model.ExperienceDescription,
                    ExperienceType = model.ExperienceType,
                    ExperienceLocation = model.ExperienceLocation,
                    ExperienceMinPrice = model.ExperienceMinPrice,
                    ExperienceMaxPrice = model.ExperienceMaxPrice,
                    ExperienceStartDate = model.ExperienceStartDate,
                    ExperienceEndDate = model.ExperienceEndDate,
                    MaxCapacity = model.MaxCapacity,
                    Lat = model.Lat,
                    Long = model.Long,
                    AddedAt = DateTime.Now
                };

                _context.Experiences.Add(experience);
                await _context.SaveChangesAsync();

                // Create experience details
                if (model.Details != null && model.Details.Any())
                {
                    foreach (var detail in model.Details)
                    {
                        var experienceDetail = new ExperienceDetail
                        {
                            ExperienceId = experience.ExperienceId,
                            ExperienceDetailDate = detail.Date,
                            ExperienceDetailTime = detail.Time,
                            ExperienceDetailPrice = detail.Price,
                            ExperienceDetailStatus = "Active",
                            AddedAt = DateTime.Now
                        };

                        _context.ExperienceDetails.Add(experienceDetail);
                    }

                    await _context.SaveChangesAsync();
                }

                // Handle image uploads
                if (model.Images != null && model.Images.Any())
                {
                    foreach (var imageFile in model.Images)
                    {
                        var path = await _fileUploadService.UploadFileAsync(imageFile, "experiences");

                        var image = new Image
                        {
                            ExperienceId = experience.ExperienceId,
                            ImageAttachment = path,
                            ImageTime = DateTime.Now,
                            AddedAt = DateTime.Now
                        };

                        _context.Images.Add(image);
                    }

                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                TempData["Success"] = "Experience created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating experience");
                TempData["Error"] = "Failed to create experience. Please try again.";
                return View("CreateEdit", model);
            }
        }

        /// <summary>
        /// Show the form for editing the specified experience
        /// </summary>
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var experience = await _context.Experiences
                    .Include(e => e.Images)
                    .Include(e => e.ExperienceDetails)
                    .FirstOrDefaultAsync(e => e.ExperienceId == id);

                if (experience == null)
                {
                    TempData["Error"] = "Experience not found.";
                    return RedirectToAction(nameof(Index));
                }

                var model = new ExperienceFormViewModel
                {
                    ExperienceId = experience.ExperienceId,
                    ExperienceTitle = experience.ExperienceTitle,
                    ExperienceDescription = experience.ExperienceDescription,
                    ExperienceType = experience.ExperienceType,
                    ExperienceLocation = experience.ExperienceLocation,
                    ExperienceMinPrice = experience.ExperienceMinPrice ?? 0,
                    ExperienceMaxPrice = experience.ExperienceMaxPrice ?? 0,
                    ExperienceStartDate = experience.ExperienceStartDate,
                    ExperienceEndDate = experience.ExperienceEndDate,
                    MaxCapacity = experience.MaxCapacity ?? 0,
                    Lat = experience.Lat ?? 0,
                    Long = experience.Long ?? 0,
                    ExistingImages = experience.Images.Select(i => new ExistingImageViewModel
                    {
                        ImageId = i.ImageId,
                        ImageAttachment = i.ImageAttachment
                    }).ToList(),
                    Details = experience.ExperienceDetails.Select(d => new ExperienceDetailViewModel
                    {
                        ExperienceDetailId = d.ExperienceDetailId,
                        Date = d.ExperienceDetailDate,
                        Time = d.ExperienceDetailTime,
                        Price = d.ExperienceDetailPrice ?? 0
                    }).ToList()
                };

                //return View("CreateEdit", model);
                return View("~/Views/Admin/Experience/CreateEdit.cshtml", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading experience for edit {ExperienceId}", id);
                TempData["Error"] = "Failed to load experience. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Update the specified experience
        /// </summary>
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ExperienceFormViewModel model)
        {
            if (id != model.ExperienceId)
            {
                TempData["Error"] = "Invalid experience ID.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                // Reload existing images for display
                var existingExp = await _context.Experiences
                    .Include(e => e.Images)
                    .FirstOrDefaultAsync(e => e.ExperienceId == id);

                if (existingExp != null)
                {
                    model.ExistingImages = existingExp.Images.Select(i => new ExistingImageViewModel
                    {
                        ImageId = i.ImageId,
                        ImageAttachment = i.ImageAttachment
                    }).ToList();
                }

                return View("~/Views/Admin/Experience/CreateEdit.cshtml", model);
                //return View("CreateEdit", model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var experience = await _context.Experiences
                    .Include(e => e.Images)
                    .Include(e => e.ExperienceDetails)
                    .FirstOrDefaultAsync(e => e.ExperienceId == id);

                if (experience == null)
                {
                    TempData["Error"] = "Experience not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Update experience
                experience.ExperienceTitle = model.ExperienceTitle;
                experience.ExperienceDescription = model.ExperienceDescription;
                experience.ExperienceType = model.ExperienceType;
                experience.ExperienceLocation = model.ExperienceLocation;
                experience.ExperienceMinPrice = model.ExperienceMinPrice;
                experience.ExperienceMaxPrice = model.ExperienceMaxPrice;
                experience.ExperienceStartDate = model.ExperienceStartDate;
                experience.ExperienceEndDate = model.ExperienceEndDate;
                experience.MaxCapacity = model.MaxCapacity;
                experience.Lat = model.Lat;
                experience.Long = model.Long;

                await _context.SaveChangesAsync();

                // Add new details (don't delete existing ones to preserve bookings)
                if (model.Details != null && model.Details.Any())
                {
                    var newDetails = model.Details.Where(d => d.ExperienceDetailId == null || d.ExperienceDetailId == 0);

                    foreach (var detail in newDetails)
                    {
                        var experienceDetail = new ExperienceDetail
                        {
                            ExperienceId = experience.ExperienceId,
                            ExperienceDetailDate = detail.Date,
                            ExperienceDetailTime = detail.Time,
                            ExperienceDetailPrice = detail.Price,
                            ExperienceDetailStatus = "Active",
                            AddedAt = DateTime.Now
                        };

                        _context.ExperienceDetails.Add(experienceDetail);
                    }

                    await _context.SaveChangesAsync();
                }

                // Handle new image uploads
                if (model.Images != null && model.Images.Any())
                {
                    foreach (var imageFile in model.Images)
                    {
                        var path = await _fileUploadService.UploadFileAsync(imageFile, "experiences");

                        var image = new Image
                        {
                            ExperienceId = experience.ExperienceId,
                            ImageAttachment = path,
                            ImageTime = DateTime.Now,
                            AddedAt = DateTime.Now
                        };

                        _context.Images.Add(image);
                    }

                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                TempData["Success"] = "Experience updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating experience {ExperienceId}", id);
                TempData["Error"] = "Failed to update experience. Please try again.";
                return View("CreateEdit", model);
            }
        }

        /// <summary>
        /// Remove the specified experience
        /// </summary>
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var experience = await _context.Experiences
                    .Include(e => e.Images)
                    .Include(e => e.ExperienceDetails)
                    .FirstOrDefaultAsync(e => e.ExperienceId == id);

                if (experience == null)
                {
                    TempData["Error"] = "Experience not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Delete image files
                foreach (var image in experience.Images)
                {
                    if (!string.IsNullOrEmpty(image.ImageAttachment))
                    {
                        try
                        {
                            await _fileUploadService.DeleteFileAsync(image.ImageAttachment);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to delete image file: {ImagePath}", image.ImageAttachment);
                        }
                    }
                }

                // Delete related records
                _context.ExperienceDetails.RemoveRange(experience.ExperienceDetails);
                _context.Images.RemoveRange(experience.Images);
                _context.Experiences.Remove(experience);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = "Experience deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error deleting experience {ExperienceId}", id);
                TempData["Error"] = "Failed to delete experience. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Delete image (AJAX)
        /// </summary>
        [HttpPost("DeleteImage/{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            try
            {
                var image = await _context.Images.FindAsync(id);

                if (image == null)
                {
                    return Json(new { success = false, message = "Image not found" });
                }

                // Delete file
                if (!string.IsNullOrEmpty(image.ImageAttachment))
                {
                    await _fileUploadService.DeleteFileAsync(image.ImageAttachment);
                }

                _context.Images.Remove(image);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Image deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image {ImageId}", id);
                return Json(new { success = false, message = "Failed to delete image" });
            }
        }
    }
}