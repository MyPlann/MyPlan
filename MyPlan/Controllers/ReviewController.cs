using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyPlan.Data;
using MyPlan.ViewModels.AdminVMs.ReviewVMs;

namespace MyPlan.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    [Route("Admin/Review")]
    public class ReviewController : Controller
    {
        private readonly MyTripDbContext _context;
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(MyTripDbContext context, ILogger<ReviewController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Display a listing of reviews
        /// </summary>
        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(ReviewFilterViewModel? filter)
        {
            try
            {
                var query = _context.Reviews
                    .Include(r => r.Visitor)
                        .ThenInclude(v => v.User)
                    .Include(r => r.Experience)
                    .Include(r => r.Booking)
                    .AsQueryable();

                // Apply filters
                if (filter != null)
                {
                    if (!string.IsNullOrEmpty(filter.SearchTerm))
                    {
                        var searchLower = filter.SearchTerm.ToLower();
                        query = query.Where(r =>
                            r.ReviewComment != null && r.ReviewComment.ToLower().Contains(searchLower) ||
                            r.Visitor.User.FullName != null && r.Visitor.User.FullName.ToLower().Contains(searchLower) ||
                            r.Experience.ExperienceTitle != null && r.Experience.ExperienceTitle.ToLower().Contains(searchLower));
                    }

                    if (filter.Rating.HasValue)
                    {
                        query = query.Where(r => r.ReviewRating == filter.Rating.Value);
                    }

                    if (filter.FromDate.HasValue)
                    {
                        query = query.Where(r => r.ReviewTime >= filter.FromDate.Value);
                    }

                    if (filter.ToDate.HasValue)
                    {
                        var toDate = filter.ToDate.Value.AddDays(1);
                        query = query.Where(r => r.ReviewTime < toDate);
                    }

                    if (!string.IsNullOrEmpty(filter.ExperienceType))
                    {
                        query = query.Where(r => r.Experience.ExperienceType == filter.ExperienceType);
                    }
                }

                // Apply sorting
                query = (filter?.SortBy ?? "date_desc") switch
                {
                    "date_asc" => query.OrderBy(r => r.AddedAt),
                    "rating_asc" => query.OrderBy(r => r.ReviewRating).ThenByDescending(r => r.AddedAt),
                    "rating_desc" => query.OrderByDescending(r => r.ReviewRating).ThenByDescending(r => r.AddedAt),
                    _ => query.OrderByDescending(r => r.AddedAt)
                };

                var reviews = await query
                    .Select(r => new ReviewListViewModel
                    {
                        ReviewId = r.ReviewId,
                        ReviewRating = r.ReviewRating ?? 0,
                        ReviewComment = r.ReviewComment,
                        ReviewTime = r.ReviewTime ?? DateTime.Now,
                        AddedAt = r.AddedAt ?? DateTime.Now,

                        VisitorName = r.Visitor.User.FullName,
                        VisitorEmail = r.Visitor.User.Email,
                        VisitorImage = r.Visitor.User.Image,

                        ExperienceId = r.Experience.ExperienceId,
                        ExperienceTitle = r.Experience.ExperienceTitle,
                        ExperienceType = r.Experience.ExperienceType,
                        ExperienceLocation = r.Experience.ExperienceLocation,

                        BookingId = r.Booking != null ? r.Booking.BookingId : (int?)null
                    })
                    .ToListAsync();

                // Calculate statistics
                var viewModel = new ReviewIndexViewModel
                {
                    Reviews = reviews,
                    Filter = filter ?? new ReviewFilterViewModel(),
                    TotalReviews = reviews.Count,
                    AverageRating = reviews.Any() ? reviews.Average(r => r.ReviewRating) : 0,
                    FiveStarCount = reviews.Count(r => r.ReviewRating == 5),
                    FourStarCount = reviews.Count(r => r.ReviewRating == 4),
                    ThreeStarCount = reviews.Count(r => r.ReviewRating == 3),
                    TwoStarCount = reviews.Count(r => r.ReviewRating == 2),
                    OneStarCount = reviews.Count(r => r.ReviewRating == 1)
                };

                return View("~/Views/Admin/Review/Index.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reviews list");
                TempData["Error"] = "Failed to load reviews. Please try again.";
                return View(new ReviewIndexViewModel());
            }
        }

        /// <summary>
        /// Show review details
        /// </summary>
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var review = await _context.Reviews
                    .Include(r => r.Visitor)
                        .ThenInclude(v => v.User)
                    .Include(r => r.Experience)
                    .Include(r => r.Booking)
                    .Where(r => r.ReviewId == id)
                    .Select(r => new ReviewDetailsViewModel
                    {
                        ReviewId = r.ReviewId,
                        ReviewRating = r.ReviewRating ?? 0,
                        ReviewComment = r.ReviewComment,
                        ReviewTime = r.ReviewTime ?? DateTime.Now,
                        AddedAt = r.AddedAt ?? DateTime.Now,

                        VisitorName = r.Visitor.User.FullName,
                        VisitorEmail = r.Visitor.User.Email,
                        VisitorPhone = r.Visitor.VisitorPhoneNumber,
                        VisitorImage = r.Visitor.User.Image,

                        ExperienceId = r.Experience.ExperienceId,
                        ExperienceTitle = r.Experience.ExperienceTitle,
                        ExperienceType = r.Experience.ExperienceType,
                        ExperienceLocation = r.Experience.ExperienceLocation,
                        ExperienceDescription = r.Experience.ExperienceDescription,

                        BookingId = r.Booking != null ? r.Booking.BookingId : (int?)null,
                        BookingDate = r.Booking != null ? r.Booking.BookingDate : (DateOnly?)null,
                        BookingStatus = r.Booking != null ? r.Booking.BookingStatus : null
                    })
                    .FirstOrDefaultAsync();

                if (review == null)
                {
                    TempData["Error"] = "Review not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving review details for review {ReviewId}", id);
                TempData["Error"] = "Failed to load review details. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Remove the specified review
        /// </summary>
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var review = await _context.Reviews.FindAsync(id);

                if (review == null)
                {
                    TempData["Error"] = "Review not found.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Review deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review {ReviewId}", id);
                TempData["Error"] = "Failed to delete review. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Bulk delete reviews
        /// </summary>
        [HttpPost("BulkDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkDelete([FromForm] int[] reviewIds)
        {
            if (reviewIds == null || reviewIds.Length == 0)
            {
                TempData["Error"] = "No reviews selected for deletion.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var reviews = await _context.Reviews
                    .Where(r => reviewIds.Contains(r.ReviewId))
                    .ToListAsync();

                if (reviews.Count == 0)
                {
                    TempData["Error"] = "No reviews found.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Reviews.RemoveRange(reviews);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"{reviews.Count} review(s) deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bulk deleting reviews");
                TempData["Error"] = "Failed to delete reviews. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Get reviews by experience (AJAX)
        /// </summary>
        [HttpGet("GetByExperience/{experienceId}")]
        public async Task<IActionResult> GetByExperience(int experienceId)
        {
            try
            {
                var reviews = await _context.Reviews
                    .Include(r => r.Visitor)
                        .ThenInclude(v => v.User)
                    .Where(r => r.ExperienceId == experienceId)
                    .OrderByDescending(r => r.ReviewTime)
                    .Select(r => new ReviewListViewModel
                    {
                        ReviewId = r.ReviewId,
                        ReviewRating = r.ReviewRating ?? 0,
                        ReviewComment = r.ReviewComment,
                        ReviewTime = r.ReviewTime ?? DateTime.Now,
                        VisitorName = r.Visitor.User.FullName,
                        VisitorImage = r.Visitor.User.Image
                    })
                    .ToListAsync();

                return Json(new { success = true, reviews });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reviews for experience {ExperienceId}", experienceId);
                return Json(new { success = false, message = "Failed to load reviews" });
            }
        }
    }
}
