using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyPlan.Data;
using MyPlan.ViewModels.HomeVMs;
using System.Diagnostics;

namespace MyPlan.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyTripDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(MyTripDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var experiences = await _context.Experiences
                .Include(e => e.Images)
                .Include(e => e.Reviews)
                .Include(e => e.ExperienceDetails)
                .Where(e => e.ExperienceEndDate >= DateOnly.FromDateTime(DateTime.Now))
                .OrderBy(e => e.ExperienceStartDate)
                .Take(6)
                .Select(e => new ExperienceSummaryVM
                {
                    Id = e.ExperienceId,
                    Title = e.ExperienceTitle,
                    Description = e.ExperienceDescription ?? string.Empty,
                    Date = e.ExperienceStartDate.HasValue ? e.ExperienceStartDate.Value.ToString("yyyy-MM-dd") : string.Empty,
                    Location = e.ExperienceLocation ?? string.Empty,
                    ImageUrl = e.Images.FirstOrDefault() != null ? e.Images.First().ImageAttachment : null,
                    Price = $"{e.ExperienceMinPrice} SAR",
                    PriceNum = e.ExperienceMinPrice,
                    Time = e.ExperienceDetails
                        .Where(ed => ed.ExperienceDetailTime != null)
                        .OrderBy(ed => ed.ExperienceDetailTime)
                        .Select(ed => ed.ExperienceDetailTime.Value.ToString("h:mm tt"))
                        .FirstOrDefault() ?? "7:00 PM", // Move the logic into the query
                    Type = e.ExperienceType ?? string.Empty,
                    MaxCapacity = e.MaxCapacity,
                    StartDate = e.ExperienceStartDate,
                    EndDate = e.ExperienceEndDate,
                    Rating = new RatingVM
                    {
                        Average = e.Reviews.Any() ? Math.Round(e.Reviews.Average(r => (double)r.ReviewRating), 1) : 0,
                        Count = e.Reviews.Count
                    },
                    ReviewCount = e.Reviews.Count
                })
                .ToListAsync();

            return View(experiences);
        }

        [HttpGet]
        public async Task<IActionResult> Map()
        {
            var experiences = await _context.Experiences
                .Where(e => !string.IsNullOrEmpty(e.ExperienceLocation))
                .Where(e => e.ExperienceStartDate >= DateOnly.FromDateTime(DateTime.Now))
                .Where(e => e.ExperienceEndDate >= DateOnly.FromDateTime(DateTime.Now))
                .Select(e => new MapExperienceVM
                {
                    Id = e.ExperienceId,
                    Title = e.ExperienceTitle,
                    Description = e.ExperienceDescription ?? string.Empty,
                    Location = e.ExperienceLocation ?? string.Empty,
                    Price = e.ExperienceMinPrice > 0 ? $"{e.ExperienceMinPrice:F2} SAR" : "مجاني",
                    IsFree = e.ExperienceMinPrice == 0,
                    StartDate = e.ExperienceStartDate,
                    EndDate = e.ExperienceEndDate,
                    Lat = e.Lat ?? GetGeocodeLocation(e.ExperienceLocation ?? "").Lat,
                    Lng = e.Long ?? GetGeocodeLocation(e.ExperienceLocation ?? "").Lng
                })
                .ToListAsync();

            return View(experiences);
        }

        // Make this method static since it's used in the LINQ query
        private static (decimal Lat, decimal Lng) GetGeocodeLocation(string location)
        {
            var locations = new Dictionary<string, (decimal Lat, decimal Lng)>
            {
                { "Boulevard Riyadh City", (24.7136m, 46.6753m) },
                { "King Abdullah Financial District", (24.8095m, 46.6417m) },
                { "At-Turaif District, Diriyah", (24.7375m, 46.5755m) },
                { "King Fahd Park", (24.6789m, 46.6895m) },
                { "Riyadh Front", (24.7914m, 46.6932m) }
            };

            return locations.ContainsKey(location) ? locations[location] : (24.7136m, 46.6753m);
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Error()
        {
            return View();
        }
    }
}