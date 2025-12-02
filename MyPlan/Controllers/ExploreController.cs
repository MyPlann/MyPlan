using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyPlan.Data;
using MyPlan.ViewModels.ExploreVMs;

namespace MyPlan.Controllers
{
    public class ExploreController : Controller
    {
        private readonly MyTripDbContext _context;
        private readonly ILogger<ExploreController> _logger;

        public ExploreController(MyTripDbContext context, ILogger<ExploreController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search, string? category, string? filter)
        {
            var experienceQuery = _context.Experiences
                .Include(e => e.Reviews)
                .Include(e => e.Images)
                .Include(e => e.Bookings)
                .AsQueryable();

            // Search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                experienceQuery = experienceQuery.Where(e =>
                    e.ExperienceTitle.Contains(search) ||
                    (e.ExperienceDescription != null && e.ExperienceDescription.Contains(search)) ||
                    (e.ExperienceLocation != null && e.ExperienceLocation.Contains(search)));
            }

            // Category filter
            if (!string.IsNullOrWhiteSpace(category))
            {
                experienceQuery = experienceQuery.Where(e => e.ExperienceType == category);
            }

            // Additional filters
            if (filter == "weekend")
            {
                var friday = GetNextDayOfWeek(DayOfWeek.Friday);
                var sunday = GetNextDayOfWeek(DayOfWeek.Sunday).AddDays(1);
                experienceQuery = experienceQuery.Where(e =>
                    e.ExperienceStartDate >= DateOnly.FromDateTime(friday) &&
                    e.ExperienceStartDate <= DateOnly.FromDateTime(sunday));
            }
            else if (filter == "near_me")
            {
                experienceQuery = experienceQuery.Where(e =>
                    e.ExperienceLocation == "Riyadh" || e.ExperienceLocation == "Jeddah");
            }
            else if (filter == "popular")
            {
                experienceQuery = experienceQuery
                    .OrderByDescending(e => e.Bookings.Count);
            }

            // Get featured events
            var featuredEvents = await experienceQuery
                .Where(e => e.ExperienceEndDate >= DateOnly.FromDateTime(DateTime.Now))
                .Select(e => new FeaturedEventVM
                {
                    Id = e.ExperienceId,
                    Title = e.ExperienceTitle,
                    Description = e.ExperienceDescription ?? string.Empty,
                    Date = e.ExperienceStartDate,
                    Location = e.ExperienceLocation ?? string.Empty,
                    ImageUrl = e.Images.FirstOrDefault() != null ? e.Images.First().ImageAttachment : "https://images.unsplash.com/photo-1647977473573-2a0c8fd3e4da?w=600&q=80",
                    Price = e.ExperienceMinPrice > 0 ? $"{e.ExperienceMinPrice} SAR" : "Free",
                    Time = e.ExperienceStartDate.HasValue ? e.ExperienceStartDate.Value.ToDateTime(TimeOnly.MinValue).ToString("h:mm tt") : "7:00 PM",
                    Rating = e.Reviews.Any() ? Math.Round(e.Reviews.Average(r => (double)r.ReviewRating), 1) : 4.5,
                    ReviewCount = e.Reviews.Count,
                    Type = e.ExperienceType ?? string.Empty
                })
                .OrderByDescending(e => e.Rating)
                .Take(8)
                .ToListAsync();

            // Get recommendations
            var recommendations = await _context.Experiences
                .Include(e => e.Images)
                .Where(e => e.ExperienceEndDate >= DateOnly.FromDateTime(DateTime.Now))
                .OrderBy(e => Guid.NewGuid())
                .Take(6)
                .Select(e => new RecommendationVM
                {
                    Id = e.ExperienceId,
                    Title = e.ExperienceTitle,
                    Location = e.ExperienceLocation ?? string.Empty,
                    Price = e.ExperienceMinPrice > 0 ? $"{e.ExperienceMinPrice} SAR" : "Free",
                    Description = e.ExperienceDescription ?? string.Empty,
                    Date = e.ExperienceStartDate,
                    ImageUrl = e.Images.FirstOrDefault() != null ? e.Images.First().ImageAttachment : "https://images.unsplash.com/photo-1719561940725-29801ebd2358?w=600&q=80",
                    Time = e.ExperienceStartDate.HasValue ? e.ExperienceStartDate.Value.ToDateTime(TimeOnly.MinValue).ToString("h:mm tt") : "7:00 PM",
                    Type = e.ExperienceType ?? string.Empty
                })
                .ToListAsync();

            // Get categories with counts - process icons after the query
            var categoriesData = await _context.Experiences
                .Where(e => e.ExperienceEndDate >= DateOnly.FromDateTime(DateTime.Now))
                .Where(e => string.IsNullOrWhiteSpace(search) ||
                    e.ExperienceTitle.Contains(search) ||
                    (e.ExperienceDescription != null && e.ExperienceDescription.Contains(search)))
                .GroupBy(e => e.ExperienceType ?? "Other")
                .Select(g => new
                {
                    Name = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            // Process categories in memory to add icons
            var categories = categoriesData.Select(c => new CategoryVM
            {
                Name = c.Name,
                Icon = GetCategoryIcon(c.Name), // Now safe to call
                Count = $"{c.Count} events"
            }).ToList();

            // Get friends highlights - process in memory
            var highlightsData = await _context.Highlights
                .Include(h => h.Visitor)
                    .ThenInclude(v => v.User)
                .Where(h => h.Visitor != null && h.Visitor.User != null)
                .OrderByDescending(h => h.HighlightTime)
                .Take(10)
                .Select(h => new
                {
                    h.HighlightId,
                    h.Visitor.VisitorFirstName,
                    h.Visitor.VisitorLastName,
                    h.Visitor.User.Image,
                    h.HighlightTitle,
                    h.HighlightImage,
                    h.HighlightContent,
                    h.HighlightTime
                })
                .ToListAsync();

            var friendsHighlights = highlightsData.Select(h => new FriendHighlightVM
            {
                Id = h.HighlightId,
                FriendName = $"{h.VisitorFirstName} {h.VisitorLastName}",
                FriendAvatar = h.Image ?? GenerateInitials($"{h.VisitorFirstName} {h.VisitorLastName}"),
                EventName = h.HighlightTitle ?? string.Empty,
                EventImage = h.HighlightImage,
                Rating = 4,
                Comment = h.HighlightContent ?? string.Empty,
                Date = GetRelativeTime(h.HighlightTime),
                Likes = new Random().Next(20, 51),
                ExperienceId = 0
            }).ToList();

            // Get all friends - process in memory
            var friendsData = await _context.Visitors
                .Include(v => v.User)
                .Include(v => v.Bookings)
                .Include(v => v.Highlights)
                .Where(v => string.IsNullOrWhiteSpace(search) ||
                    v.VisitorFirstName.Contains(search) ||
                    v.VisitorLastName.Contains(search) ||
                    (v.Bio != null && v.Bio.Contains(search)))
                .Take(100)
                .Select(v => new
                {
                    v.VisitorId,
                    v.VisitorFirstName,
                    v.VisitorLastName,
                    v.User.Image,
                    v.Bio,
                    BookingsCount = v.Bookings.Count(b => b.BookingStatus == "Confirmed")
                })
                .ToListAsync();

            var allFriends = friendsData.Select(v => new FriendVM
            {
                Id = v.VisitorId,
                Name = $"{v.VisitorFirstName} {v.VisitorLastName}",
                Username = $"{v.VisitorFirstName}.{v.VisitorLastName}".ToLower(),
                Avatar = v.Image,
                Bio = v.Bio ?? "Event enthusiast exploring Saudi experiences",
                EventsAttended = v.BookingsCount,
                FriendsCount = new Random().Next(50, 301)
            }).ToList();

            var viewModel = new ExploreIndexVM
            {
                FeaturedEvents = featuredEvents,
                Recommendations = recommendations,
                Categories = categories,
                FriendsHighlights = friendsHighlights,
                AllFriends = allFriends,
                SearchQuery = search,
                Category = category,
                Filter = filter
            };

            return View(viewModel);
        }

        // Make these methods static since they're used in LINQ queries
        private static DateTime GetNextDayOfWeek(DayOfWeek day)
        {
            var today = DateTime.Today;
            var daysUntil = ((int)day - (int)today.DayOfWeek + 7) % 7;
            return today.AddDays(daysUntil);
        }

        // These methods can remain instance methods since they're called after database queries
        private string GetCategoryIcon(string category)
        {
            var icons = new Dictionary<string, string>
            {
                { "Music", "🎵" },
                { "Tech", "💻" },
                { "Sports", "⚽" },
                { "Cultural", "🎭" },
                { "Food", "🍽️" },
                { "Art", "🎨" },
                { "Business", "💼" },
                { "Education", "📚" }
            };
            return icons.ContainsKey(category) ? icons[category] : "🎪";
        }

        private string GenerateInitials(string name)
        {
            var words = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var initials = "";
            foreach (var word in words)
            {
                if (word.Length > 0)
                    initials += char.ToUpper(word[0]);
            }
            return initials;
        }

        private string GetRelativeTime(DateTime? dateTime)
        {
            if (!dateTime.HasValue) return string.Empty;

            var timeSpan = DateTime.Now - dateTime.Value;
            if (timeSpan.TotalDays >= 365)
                return $"{(int)(timeSpan.TotalDays / 365)} years ago";
            if (timeSpan.TotalDays >= 30)
                return $"{(int)(timeSpan.TotalDays / 30)} months ago";
            if (timeSpan.TotalDays >= 7)
                return $"{(int)(timeSpan.TotalDays / 7)} weeks ago";
            if (timeSpan.TotalDays >= 1)
                return $"{(int)timeSpan.TotalDays} days ago";
            if (timeSpan.TotalHours >= 1)
                return $"{(int)timeSpan.TotalHours} hours ago";
            if (timeSpan.TotalMinutes >= 1)
                return $"{(int)timeSpan.TotalMinutes} minutes ago";

            return "just now";
        }
    }
}