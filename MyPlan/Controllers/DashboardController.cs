using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyPlan.Data;
using MyPlan.Shared;
using MyPlan.ViewModels.AdminVMs.DashboardVMs;

namespace MyPlan.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    [Route("Admin/Dashboard")]
    public class DashboardController : Controller
    {
        private readonly MyTripDbContext _context;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(MyTripDbContext context, ILogger<DashboardController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var viewModel = new DashboardViewModel();

                // Get dashboard statistics
                viewModel.TotalRevenue = await _context.Payments
                    .Where(p => p.PaymentStatus == StatusHelper.PaymentPaid)
                    .SumAsync(p => p.PaymentAmount ?? 0);

                viewModel.TotalEvents = await _context.Experiences.CountAsync();

                viewModel.TotalUsers = await _context.Users
                    .Where(u => u.Role == StatusHelper.RoleVisitor)
                    .CountAsync();

                viewModel.ActiveBookings = await _context.Bookings
                    .Where(b => b.BookingStatus == StatusHelper.BookingConfirmed)
                    .CountAsync();

                // Recent bookings (last 10)
                viewModel.RecentBookings = await _context.Bookings
                    .Include(b => b.Visitor)
                        .ThenInclude(v => v.User)
                    .Include(b => b.Experience)
                    .OrderByDescending(b => b.AddedAt)
                    .Take(10)
                    .Select(b => new RecentBookingViewModel
                    {
                        BookingId = b.BookingId,
                        BookingDate = b.BookingDate,
                        BookingStatus = b.BookingStatus,
                        TotalAmount = b.TotalAmount ?? 0,
                        VisitorName = b.Visitor.User.FullName,
                        VisitorEmail = b.Visitor.User.Email,
                        ExperienceTitle = b.Experience.ExperienceTitle,
                        AddedAt = b.AddedAt ?? DateTime.Now
                    })
                    .ToListAsync();

                // Revenue data (last 12 months grouped by month)
                var revenueData = await _context.Payments
                    .Where(p => p.PaymentStatus == StatusHelper.PaymentPaid)
                    .GroupBy(p => new
                    {
                        Year = p.AddedAt.HasValue ? p.AddedAt.Value.Year : DateTime.Now.Year,
                        Month = p.AddedAt.HasValue ? p.AddedAt.Value.Month : DateTime.Now.Month
                    })
                    .Select(g => new MonthlyRevenueViewModel
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Revenue = g.Sum(p => p.PaymentAmount ?? 0)
                    })
                    .OrderBy(r => r.Year)
                    .ThenBy(r => r.Month)
                    .ToListAsync();

                viewModel.RevenueData = revenueData;

                // Event categories data
                var eventCategories = await _context.Experiences
                    .GroupBy(e => e.ExperienceType)
                    .Select(g => new EventCategoryViewModel
                    {
                        CategoryName = g.Key ?? "Other",
                        Count = g.Count()
                    })
                    .ToListAsync();

                viewModel.EventCategories = eventCategories;

                // Calculate growth percentages
                viewModel.RevenueGrowth = await CalculateRevenueGrowthAsync();
                viewModel.BookingGrowth = await CalculateBookingGrowthAsync();
                viewModel.UserGrowth = await CalculateUserGrowthAsync();

                return View("~/Views/Admin/Dashboard.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard");
                TempData["Error"] = "Failed to load dashboard data. Please try again.";
                return View(new DashboardViewModel());
            }
        }

        #region Helper Methods

        private async Task<decimal> CalculateRevenueGrowthAsync()
        {
            try
            {
                var currentMonth = DateTime.Now.Month;
                var currentYear = DateTime.Now.Year;
                var previousMonth = DateTime.Now.AddMonths(-1).Month;
                var previousYear = DateTime.Now.AddMonths(-1).Year;

                var currentMonthRevenue = await _context.Payments
                    .Where(p => p.PaymentStatus == StatusHelper.PaymentPaid
                        && p.AddedAt.HasValue
                        && p.AddedAt.Value.Month == currentMonth
                        && p.AddedAt.Value.Year == currentYear)
                    .SumAsync(p => p.PaymentAmount ?? 0);

                var previousMonthRevenue = await _context.Payments
                    .Where(p => p.PaymentStatus == StatusHelper.PaymentPaid
                        && p.AddedAt.HasValue
                        && p.AddedAt.Value.Month == previousMonth
                        && p.AddedAt.Value.Year == previousYear)
                    .SumAsync(p => p.PaymentAmount ?? 0);

                if (previousMonthRevenue == 0)
                    return currentMonthRevenue > 0 ? 100 : 0;

                return ((currentMonthRevenue - previousMonthRevenue) / previousMonthRevenue) * 100;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating revenue growth");
                return 0;
            }
        }

        private async Task<decimal> CalculateBookingGrowthAsync()
        {
            try
            {
                var currentMonth = DateTime.Now.Month;
                var currentYear = DateTime.Now.Year;
                var previousMonth = DateTime.Now.AddMonths(-1).Month;
                var previousYear = DateTime.Now.AddMonths(-1).Year;

                var currentMonthBookings = await _context.Bookings
                    .Where(b => b.AddedAt.HasValue
                        && b.AddedAt.Value.Month == currentMonth
                        && b.AddedAt.Value.Year == currentYear)
                    .CountAsync();

                var previousMonthBookings = await _context.Bookings
                    .Where(b => b.AddedAt.HasValue
                        && b.AddedAt.Value.Month == previousMonth
                        && b.AddedAt.Value.Year == previousYear)
                    .CountAsync();

                if (previousMonthBookings == 0)
                    return currentMonthBookings > 0 ? 100 : 0;

                return ((decimal)(currentMonthBookings - previousMonthBookings) / previousMonthBookings) * 100;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating booking growth");
                return 0;
            }
        }

        private async Task<decimal> CalculateUserGrowthAsync()
        {
            try
            {
                var currentMonth = DateTime.Now.Month;
                var currentYear = DateTime.Now.Year;
                var previousMonth = DateTime.Now.AddMonths(-1).Month;
                var previousYear = DateTime.Now.AddMonths(-1).Year;

                var currentMonthUsers = await _context.Users
                    .Where(u => u.Role == StatusHelper.RoleVisitor
                        && u.AddedAt.HasValue
                        && u.AddedAt.Value.Month == currentMonth
                        && u.AddedAt.Value.Year == currentYear)
                    .CountAsync();

                var previousMonthUsers = await _context.Users
                    .Where(u => u.Role == StatusHelper.RoleVisitor
                        && u.AddedAt.HasValue
                        && u.AddedAt.Value.Month == previousMonth
                        && u.AddedAt.Value.Year == previousYear)
                    .CountAsync();

                if (previousMonthUsers == 0)
                    return currentMonthUsers > 0 ? 100 : 0;

                return ((decimal)(currentMonthUsers - previousMonthUsers) / previousMonthUsers) * 100;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating user growth");
                return 0;
            }
        }

        #endregion
    }
}
