using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MyPlan.Data;
//using MyPlan.Middleware;
using MyPlan.Shared;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

#region Database Configuration
builder.Services.AddDbContext<MyTripDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("MyPlanConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()));
#endregion

#region Authentication & Authorization
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
    options.AddPolicy("VisitorOnly", policy => policy.RequireRole("visitor"));
    options.AddPolicy("AuthenticatedUser", policy => policy.RequireAuthenticatedUser());
});

#endregion

#region Session Configuration
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(120);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});
#endregion

#region Register Custom Services
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
#endregion

#region HttpContextAccessor for accessing HttpContext in services
builder.Services.AddHttpContextAccessor();
#endregion

#region Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var username = context.User.Identity?.Name ?? context.Request.Headers.Host.ToString();
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: username,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 60,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            });
    });

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", token);
    };
});
#endregion

#region Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddEventSourceLogger();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

// Map Controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Additional routes
app.MapControllerRoute(
    name: "profile",
    pattern: "profile",
    defaults: new { controller = "Profile", action = "Show" });

app.MapControllerRoute(
    name: "explore",
    pattern: "explore",
    defaults: new { controller = "Explore", action = "Index" });

app.MapControllerRoute(
    name: "booking",
    pattern: "booking/{action}/{id?}",
    defaults: new { controller = "Booking" });

app.Run();