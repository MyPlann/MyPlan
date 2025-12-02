using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyPlan.Data;
using MyPlan.Shared;
using MyPlan.ViewModels.AdminVMs.HighlightVMs;

namespace MyPlan.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    [Route("Admin/Highlight")]
    public class HighlightController : Controller
    {
        private readonly MyTripDbContext _context;
        private readonly ILogger<HighlightController> _logger;
        private readonly IFileUploadService _fileUploadService;

        public HighlightController(
            MyTripDbContext context,
            ILogger<HighlightController> logger,
            IFileUploadService fileUploadService)
        {
            _context = context;
            _logger = logger;
            _fileUploadService = fileUploadService;
        }

        /// <summary>
        /// Display a listing of highlights
        /// </summary>
        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var highlights = await _context.Highlights
                    .Include(h => h.Admin)
                        .ThenInclude(a => a.User)
                    .Include(h => h.Visitor)
                        .ThenInclude(v => v.User)
                    .OrderByDescending(h => h.AddedAt)
                    .Select(h => new HighlightListViewModel
                    {
                        HighlightId = h.HighlightId,
                        HighlightTitle = h.HighlightTitle,
                        HighlightDescription = h.HighlightDescription,
                        HighlightImage = h.HighlightImage,
                        HighlightTime = h.HighlightTime ?? DateTime.Now,
                        AddedAt = h.AddedAt ?? DateTime.Now,

                        // Admin or Visitor info
                        CreatedBy = h.Admin != null
                            ? h.Admin.User.FullName
                            : h.Visitor != null
                                ? h.Visitor.User.FullName
                                : "Unknown",
                        CreatedByType = h.Admin != null ? "Admin" : "Visitor",
                        CreatedByEmail = h.Admin != null
                            ? h.Admin.User.Email
                            : h.Visitor != null
                                ? h.Visitor.User.Email
                                : null
                    })
                    .ToListAsync();

                //return View(highlights);
                return View("~/Views/Admin/Highlight/Index.cshtml", highlights);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving highlights list");
                TempData["Error"] = "Failed to load highlights. Please try again.";
                return View(new List<HighlightListViewModel>());
            }
        }

        /// <summary>
        /// Show highlight details
        /// </summary>
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var highlight = await _context.Highlights
                    .Include(h => h.Admin)
                        .ThenInclude(a => a.User)
                    .Include(h => h.Visitor)
                        .ThenInclude(v => v.User)
                    .Where(h => h.HighlightId == id)
                    .Select(h => new HighlightDetailsViewModel
                    {
                        HighlightId = h.HighlightId,
                        HighlightTitle = h.HighlightTitle,
                        HighlightContent = h.HighlightContent,
                        HighlightDescription = h.HighlightDescription,
                        HighlightImage = h.HighlightImage,
                        HighlightTime = h.HighlightTime ?? DateTime.Now,
                        AddedAt = h.AddedAt ?? DateTime.Now,

                        CreatedBy = h.Admin != null
                            ? h.Admin.User.FullName
                            : h.Visitor != null
                                ? h.Visitor.User.FullName
                                : "Unknown",
                        CreatedByType = h.Admin != null ? "Admin" : "Visitor",
                        CreatedByEmail = h.Admin != null
                            ? h.Admin.User.Email
                            : h.Visitor != null
                                ? h.Visitor.User.Email
                                : null,
                        CreatedByImage = h.Admin != null
                            ? h.Admin.User.Image
                            : h.Visitor != null
                                ? h.Visitor.User.Image
                                : null
                    })
                    .FirstOrDefaultAsync();

                if (highlight == null)
                {
                    TempData["Error"] = "Highlight not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(highlight);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving highlight details for highlight {HighlightId}", id);
                TempData["Error"] = "Failed to load highlight details. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Remove the specified highlight
        /// </summary>
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var highlight = await _context.Highlights.FindAsync(id);

                if (highlight == null)
                {
                    TempData["Error"] = "Highlight not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Delete associated image file if exists
                if (!string.IsNullOrEmpty(highlight.HighlightImage))
                {
                    try
                    {
                        await _fileUploadService.DeleteFileAsync(highlight.HighlightImage);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to delete highlight image file: {ImagePath}",
                            highlight.HighlightImage);
                        // Continue with database deletion even if file deletion fails
                    }
                }

                _context.Highlights.Remove(highlight);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Highlight deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting highlight {HighlightId}", id);
                TempData["Error"] = "Failed to delete highlight. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Bulk delete highlights
        /// </summary>
        [HttpPost("BulkDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkDelete([FromForm] int[] highlightIds)
        {
            if (highlightIds == null || highlightIds.Length == 0)
            {
                TempData["Error"] = "No highlights selected for deletion.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var highlights = await _context.Highlights
                    .Where(h => highlightIds.Contains(h.HighlightId))
                    .ToListAsync();

                if (highlights.Count == 0)
                {
                    TempData["Error"] = "No highlights found.";
                    return RedirectToAction(nameof(Index));
                }

                // Delete associated image files
                foreach (var highlight in highlights)
                {
                    if (!string.IsNullOrEmpty(highlight.HighlightImage))
                    {
                        try
                        {
                            await _fileUploadService.DeleteFileAsync(highlight.HighlightImage);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex,
                                "Failed to delete highlight image file: {ImagePath}",
                                highlight.HighlightImage);
                        }
                    }
                }

                _context.Highlights.RemoveRange(highlights);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"{highlights.Count} highlight(s) deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bulk deleting highlights");
                TempData["Error"] = "Failed to delete highlights. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
