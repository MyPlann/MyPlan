using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyPlan.Data;
using MyPlan.Shared;
using MyPlan.ViewModels.ItineraryVMs;

namespace MyPlan.Controllers
{
    [Authorize(Roles = "visitor")]
    public class ItineraryController : Controller
    {
        private readonly MyTripDbContext _context;
        private readonly ILogger<ItineraryController> _logger;

        public ItineraryController(MyTripDbContext context, ILogger<ItineraryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(CreateItineraryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please correct the errors and try again.";
                TempData["OpenItineraryModal"] = true;
                return RedirectToAction("Show", "Profile");
            }

            // Validate end date is after or equal to start date
            if (model.EndDate < model.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date must be after or equal to start date");
                TempData["Error"] = "End date must be after or equal to start date.";
                TempData["OpenItineraryModal"] = true;
                return RedirectToAction("Show", "Profile");
            }

            try
            {
                var userId = User.GetUserId();
                var visitor = await _context.Visitors.FirstOrDefaultAsync(v => v.UserId == userId);

                if (visitor == null)
                {
                    TempData["Error"] = "Visitor profile not found.";
                    return RedirectToAction("Show", "Profile");
                }

                var itinerary = new Models.Itinerary
                {
                    VisitorId = visitor.VisitorId,
                    ExperienceId = model.ExperienceId,
                    ItineraryStartDate = model.StartDate,
                    ItineraryEndDate = model.EndDate,
                    ItineraryDay = model.Day,
                    ItineraryDescription = model.Description,
                    AddedAt = DateTime.Now
                };

                _context.Itineraries.Add(itinerary);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Itinerary added successfully!";
                return RedirectToAction("Show", "Profile");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating itinerary");
                TempData["Error"] = "An error occurred while creating the itinerary.";
                TempData["OpenItineraryModal"] = true;
                return RedirectToAction("Show", "Profile");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateItineraryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please correct the errors and try again.";
                TempData["OpenItineraryModal"] = model.ItineraryId;
                return RedirectToAction("Show", "Profile");
            }

            // Validate end date
            if (model.EndDate < model.StartDate)
            {
                TempData["Error"] = "End date must be after or equal to start date.";
                TempData["OpenItineraryModal"] = model.ItineraryId;
                return RedirectToAction("Show", "Profile");
            }

            try
            {
                var userId = User.GetUserId();
                var visitor = await _context.Visitors.FirstOrDefaultAsync(v => v.UserId == userId);

                var itinerary = await _context.Itineraries
                    .FirstOrDefaultAsync(i => i.ItineraryId == model.ItineraryId && i.VisitorId == visitor!.VisitorId);

                if (itinerary == null)
                {
                    TempData["Error"] = "Itinerary not found.";
                    return RedirectToAction("Show", "Profile");
                }

                itinerary.ExperienceId = model.ExperienceId;
                itinerary.ItineraryStartDate = model.StartDate;
                itinerary.ItineraryEndDate = model.EndDate;
                itinerary.ItineraryDay = model.Day;
                itinerary.ItineraryDescription = model.Description;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Itinerary updated successfully!";
                return RedirectToAction("Show", "Profile");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating itinerary {ItineraryId}", model.ItineraryId);
                TempData["Error"] = "An error occurred while updating the itinerary.";
                TempData["OpenItineraryModal"] = model.ItineraryId;
                return RedirectToAction("Show", "Profile");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = User.GetUserId();
                var visitor = await _context.Visitors.FirstOrDefaultAsync(v => v.UserId == userId);

                var itinerary = await _context.Itineraries
                    .FirstOrDefaultAsync(i => i.ItineraryId == id && i.VisitorId == visitor!.VisitorId);

                if (itinerary == null)
                {
                    TempData["Error"] = "Itinerary not found.";
                    return RedirectToAction("Show", "Profile");
                }

                _context.Itineraries.Remove(itinerary);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Itinerary deleted successfully!";
                return RedirectToAction("Show", "Profile");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting itinerary {ItineraryId}", id);
                TempData["Error"] = "An error occurred while deleting the itinerary.";
                return RedirectToAction("Show", "Profile");
            }
        }
    }
}
