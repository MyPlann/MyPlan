using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyPlan.Data;
using MyPlan.Shared;
using MyPlan.ViewModels.AdminVMs.ReportVMs;

namespace MyPlan.Controllers.Admin
{
    [Authorize(Roles = "admin")]
    [Route("Admin/Report")]
    public class ReportController : Controller
    {
        private readonly MyTripDbContext _context;
        private readonly ILogger<ReportController> _logger;

        public ReportController(MyTripDbContext context, ILogger<ReportController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Display analytics dashboard
        /// </summary>
        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var endDate = DateTime.Now;
                var startDate = endDate.AddDays(-30);

                var viewModel = new ReportIndexViewModel
                {
                    StartDate = startDate,
                    EndDate = endDate
                };

                // Overall Statistics
                viewModel.TotalRevenue = await _context.Payments
                    .Where(p => p.PaymentStatus == StatusHelper.PaymentPaid)
                    .SumAsync(p => p.PaymentAmount ?? 0);

                viewModel.TotalBookings = await _context.Bookings.CountAsync();
                viewModel.TotalUsers = await _context.Users
                    .Where(u => u.Role == StatusHelper.RoleVisitor)
                    .CountAsync();
                viewModel.TotalExperiences = await _context.Experiences.CountAsync();
                viewModel.TotalReviews = await _context.Reviews.CountAsync();
                viewModel.TotalHighlights = await _context.Highlights.CountAsync();

                // Growth Statistics
                viewModel.RevenueGrowth = await CalculateRevenueGrowthAsync();
                viewModel.BookingGrowth = await CalculateBookingGrowthAsync();
                viewModel.UserGrowth = await CalculateUserGrowthAsync();

                // Monthly Revenue Data (Last 12 months)
                viewModel.MonthlyRevenueData = await GetMonthlyRevenueDataAsync();

                // Experience Categories Distribution
                viewModel.ExperienceCategories = await _context.Experiences
                    .GroupBy(e => e.ExperienceType)
                    .Select(g => new CategoryDataViewModel
                    {
                        Category = g.Key ?? "Other",
                        Count = g.Count()
                    })
                    .ToListAsync();

                // Booking Status Distribution
                viewModel.BookingStatuses = await _context.Bookings
                    .GroupBy(b => b.BookingStatus)
                    .Select(g => new StatusDataViewModel
                    {
                        Status = g.Key ?? "Unknown",
                        Count = g.Count()
                    })
                    .ToListAsync();

                // Recent Bookings
                viewModel.RecentBookings = await _context.Bookings
                    .Include(b => b.Visitor)
                        .ThenInclude(v => v.User)
                    .Include(b => b.Experience)
                    .OrderByDescending(b => b.AddedAt)
                    .Take(5)
                    .Select(b => new RecentActivityViewModel
                    {
                        Id = b.BookingId,
                        Type = "Booking",
                        Description = $"{b.Visitor.User.FullName} booked {b.Experience.ExperienceTitle}",
                        Amount = b.TotalAmount ?? 0,
                        Date = b.AddedAt ?? DateTime.Now
                    })
                    .ToListAsync();

                // Recent Payments
                viewModel.RecentPayments = await _context.Payments
                    .Include(p => p.Booking)
                        .ThenInclude(b => b.Visitor)
                            .ThenInclude(v => v.User)
                    .Where(p => p.PaymentStatus == StatusHelper.PaymentPaid)
                    .OrderByDescending(p => p.PaymentDate)
                    .Take(5)
                    .Select(p => new RecentActivityViewModel
                    {
                        Id = p.PaymentId,
                        Type = "Payment",
                        Description = $"Payment from {p.Booking.Visitor.User.FullName}",
                        Amount = p.PaymentAmount ?? 0,
                        Date = p.AddedAt ?? DateTime.Now
                    })
                    .ToListAsync();

                // Top Experiences by Revenue
                viewModel.TopExperiences = await GetTopExperiencesByRevenueAsync();

                // User Registration Trends (Last 30 days)
                viewModel.UserRegistrations = await GetUserRegistrationTrendsAsync(startDate, endDate);

                return View("~/Views/Admin/Report/Index.cshtml", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading report dashboard");
                TempData["Error"] = "Failed to load report data. Please try again.";
                return View(new ReportIndexViewModel());
            }
        }

        /// <summary>
        /// Generate custom reports
        /// </summary>
        [HttpPost("Generate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Generate(GenerateReportViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid report parameters.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                return model.ReportType switch
                {
                    "revenue" => await GenerateRevenueReport(model.StartDate, model.EndDate),
                    "bookings" => await GenerateBookingsReport(model.StartDate, model.EndDate),
                    "users" => await GenerateUsersReport(model.StartDate, model.EndDate),
                    "experiences" => await GenerateExperiencesReport(model.StartDate, model.EndDate),
                    _ => RedirectToAction(nameof(Index))
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating report: {ReportType}", model.ReportType);
                TempData["Error"] = "Failed to generate report. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        #region Report Generation Methods

        private async Task<IActionResult> GenerateRevenueReport(DateTime startDate, DateTime endDate)
        {
            var revenueData = await _context.Payments
                .Where(p => p.PaymentStatus == StatusHelper.PaymentPaid
                    && p.PaymentDate.HasValue
                    && p.PaymentDate >= DateOnly.FromDateTime(startDate)
                    && p.PaymentDate <= DateOnly.FromDateTime(endDate))
                .GroupBy(p => p.PaymentDate)
                .Select(g => new DailyRevenueViewModel
                {
                    Date = g.Key ?? DateOnly.FromDateTime(DateTime.Now),
                    Revenue = g.Sum(p => p.PaymentAmount ?? 0),
                    TransactionCount = g.Count()
                })
                .OrderBy(d => d.Date)
                .ToListAsync();

            var viewModel = new RevenueReportViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                DailyRevenue = revenueData,
                TotalRevenue = revenueData.Sum(d => d.Revenue),
                AverageRevenue = revenueData.Any() ? revenueData.Average(d => d.Revenue) : 0,
                TotalTransactions = revenueData.Sum(d => d.TransactionCount)
            };

            return View("RevenueReport", viewModel);
        }

        private async Task<IActionResult> GenerateBookingsReport(DateTime startDate, DateTime endDate)
        {
            var bookingsData = await _context.Bookings
                .Where(b => b.BookingDate >= DateOnly.FromDateTime(startDate)
                    && b.BookingDate <= DateOnly.FromDateTime(endDate))
                .GroupBy(b => b.BookingDate)
                .Select(g => new DailyBookingViewModel
                {
                    Date = g.Key,
                    BookingCount = g.Count(),
                    Revenue = g.Sum(b => b.TotalAmount ?? 0),
                    TicketCount = g.Sum(b => b.BookingNumberOfTicket ?? 0)
                })
                .OrderBy(d => d.Date)
                .ToListAsync();

            var viewModel = new BookingsReportViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                DailyBookings = bookingsData,
                TotalBookings = bookingsData.Sum(d => d.BookingCount),
                TotalRevenue = bookingsData.Sum(d => d.Revenue),
                TotalTickets = bookingsData.Sum(d => d.TicketCount),
                AverageBookingsPerDay = bookingsData.Any() ? bookingsData.Average(d => d.BookingCount) : 0
            };

            return View("BookingsReport", viewModel);
        }

        private async Task<IActionResult> GenerateUsersReport(DateTime startDate, DateTime endDate)
        {
            var usersData = await _context.Users
                .Where(u => u.Role == StatusHelper.RoleVisitor
                    && u.AddedAt.HasValue
                    && u.AddedAt >= startDate
                    && u.AddedAt <= endDate)
                .GroupBy(u => u.AddedAt.Value.Date)
                .Select(g => new DailyUserViewModel
                {
                    Date = DateOnly.FromDateTime(g.Key),
                    UserCount = g.Count()
                })
                .OrderBy(d => d.Date)
                .ToListAsync();

            var viewModel = new UsersReportViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                DailyUsers = usersData,
                TotalUsers = usersData.Sum(d => d.UserCount),
                AverageUsersPerDay = usersData.Any() ? usersData.Average(d => d.UserCount) : 0
            };

            return View("UsersReport", viewModel);
        }

        private async Task<IActionResult> GenerateExperiencesReport(DateTime startDate, DateTime endDate)
        {
            var experiencesData = await _context.Experiences
                .Where(e => e.AddedAt.HasValue
                    && e.AddedAt >= startDate
                    && e.AddedAt <= endDate)
                .Select(e => new ExperienceReportViewModel
                {
                    ExperienceId = e.ExperienceId,
                    ExperienceTitle = e.ExperienceTitle,
                    ExperienceType = e.ExperienceType,
                    ExperienceLocation = e.ExperienceLocation,
                    BookingCount = e.Bookings.Count,
                    ReviewCount = e.Reviews.Count,
                    AverageRating = e.Reviews.Any() ? e.Reviews.Average(r => r.ReviewRating ?? 0) : 0,
                    TotalRevenue = e.Bookings
                        .Where(b => b.Payments.Any(p => p.PaymentStatus == StatusHelper.PaymentPaid))
                        .Sum(b => b.TotalAmount ?? 0),
                    AddedAt = e.AddedAt ?? DateTime.Now
                })
                .OrderByDescending(e => e.TotalRevenue)
                .ToListAsync();

            var viewModel = new ExperiencesReportViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                Experiences = experiencesData,
                TotalExperiences = experiencesData.Count,
                TotalRevenue = experiencesData.Sum(e => e.TotalRevenue),
                TotalBookings = experiencesData.Sum(e => e.BookingCount)
            };

            return View("ExperiencesReport", viewModel);
        }

        #endregion

        #region Helper Methods

        private async Task<decimal> CalculateRevenueGrowthAsync()
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

        private async Task<decimal> CalculateBookingGrowthAsync()
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

        private async Task<decimal> CalculateUserGrowthAsync()
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

        private async Task<List<MonthlyRevenueDataViewModel>> GetMonthlyRevenueDataAsync()
        {
            var currentYear = DateTime.Now.Year;

            var data = await _context.Payments
                .Where(p => p.PaymentStatus == StatusHelper.PaymentPaid
                    && p.AddedAt.HasValue
                    && p.AddedAt.Value.Year == currentYear)
                .GroupBy(p => p.AddedAt.Value.Month)
                .Select(g => new MonthlyRevenueDataViewModel
                {
                    Month = g.Key,
                    Revenue = g.Sum(p => p.PaymentAmount ?? 0)
                })
                .ToListAsync();

            // Fill missing months with 0
            var result = new List<MonthlyRevenueDataViewModel>();
            for (int i = 1; i <= 12; i++)
            {
                var monthData = data.FirstOrDefault(d => d.Month == i);
                result.Add(new MonthlyRevenueDataViewModel
                {
                    Month = i,
                    Revenue = monthData?.Revenue ?? 0
                });
            }

            return result;
        }

        private async Task<List<TopExperienceViewModel>> GetTopExperiencesByRevenueAsync()
        {
            return await _context.Experiences
                .Select(e => new TopExperienceViewModel
                {
                    ExperienceId = e.ExperienceId,
                    ExperienceTitle = e.ExperienceTitle,
                    ExperienceType = e.ExperienceType,
                    TotalBookings = e.Bookings.Count,
                    TotalRevenue = e.Bookings
                        .Where(b => b.Payments.Any(p => p.PaymentStatus == StatusHelper.PaymentPaid))
                        .Sum(b => b.TotalAmount ?? 0)
                })
                .OrderByDescending(e => e.TotalRevenue)
                .Take(5)
                .ToListAsync();
        }

        private async Task<List<UserRegistrationTrendViewModel>> GetUserRegistrationTrendsAsync(
            DateTime startDate, DateTime endDate)
        {
            return await _context.Users
                .Where(u => u.Role == StatusHelper.RoleVisitor
                    && u.AddedAt.HasValue
                    && u.AddedAt >= startDate
                    && u.AddedAt <= endDate)
                .GroupBy(u => u.AddedAt.Value.Date)
                .Select(g => new UserRegistrationTrendViewModel
                {
                    Date = DateOnly.FromDateTime(g.Key),
                    Count = g.Count()
                })
                .OrderBy(t => t.Date)
                .ToListAsync();
        }

        #endregion
    }
}