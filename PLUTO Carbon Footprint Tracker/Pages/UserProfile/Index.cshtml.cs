using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PLUTO_Carbon_Footprint_Tracker.Data;
using PLUTO_Carbon_Footprint_Tracker.Services;
using PLUTO_Carbon_Footprint_Tracker.ViewModels;

namespace PLUTO_Carbon_Footprint_Tracker.Pages.UserProfile
{
    public class IndexModel : PageModel
    {
        private readonly CFTDBContext _context;
        private readonly SyncUserDataService _syncUserDataService;

        public IndexModel(CFTDBContext context, SyncUserDataService syncUserDataService)
        {
            _context = context;
            _syncUserDataService = syncUserDataService;
        }

        public User CurrentUser { get; set; } = default!;

        public int TotalEntriesLogged { get; set; }
        public decimal TotalCarbonLogged { get; set; }
        public string? HighestCarbonActivityType { get; set; }
        public decimal WeeklyTotal { get; set; }
        public decimal MonthlyTotal { get; set; }
        public DateTime? BestDayDate { get; set; }
        public decimal BestDayCarbon { get; set; }
        public decimal YearTotal { get; set; }
        public IList<MonthlyEmissions> MonthlyEmissions { get; set; } = new List<MonthlyEmissions>();
        public async Task OnGetAsync()
        {
            // Ensure the user exists in the database and retrieve their data
            var user = await _syncUserDataService.EnsureUserExistsAsync(User);
            if (user == null)
            {
                return;
            }

            // Set the current user for use in the Razor Page
            CurrentUser = user;

            // Get today's date for filtering entries
            var today = DateTime.UtcNow.Date;

            // Load the user's emission entries along with their activities and activity types
            var userEntries = await _context.EmissionEntries.Where(e => e.UserId == user.Id)
                .Include(e => e.Activity).ThenInclude(a => a.ActivityType).ToListAsync();

            // Calculate the total number of entries and total carbon logged
            TotalEntriesLogged = userEntries.Count;
            TotalCarbonLogged = userEntries.Sum(e => e.CarbonKg);

            // Determine the activity type with the highest total carbon emissions
            var highestCarbonActivity = userEntries
                .GroupBy(e => e.Activity.ActivityType.Name)
                .Select(g => new
                {
                    ActivityTypeName = g.Key,
                    TotalCarbon = g.Sum(e => e.CarbonKg)
                })
                .OrderByDescending(g => g.TotalCarbon).FirstOrDefault();

            // If there is a highest carbon activity, set the corresponding property
            if (highestCarbonActivity != null)
            {
                HighestCarbonActivityType = highestCarbonActivity.ActivityTypeName;
            }

            // Calculate weekly, monthly, and yearly totals
            WeeklyTotal = userEntries.Where(e => e.EntryDate.Date >= today.AddDays(-6))
                .Sum(e => e.CarbonKg);

            MonthlyTotal = userEntries.Where(e => e.EntryDate.Month == today.Month &&
            e.EntryDate.Year == today.Year).Sum(e => e.CarbonKg);

            YearTotal = userEntries.Where(e => e.EntryDate.Year == today.Year)
                .Sum(e => e.CarbonKg);

            // Identify the best day (the day with the lowest total carbon emissions)
            var bestDay = userEntries.GroupBy(e => e.EntryDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalCarbon = g.Sum(e => e.CarbonKg)
                })
                .OrderBy(g => g.TotalCarbon).FirstOrDefault();

            // If there is a best day, set the corresponding properties
            if (bestDay != null)
            {
                BestDayDate = bestDay.Date;
                BestDayCarbon = bestDay.TotalCarbon;
            }
            // Prepare monthly emissions data for the current year
            MonthlyEmissions = userEntries.Where(e => e.EntryDate.Year == today.Year)
                .GroupBy(e => e.EntryDate.Month)
                .Select(g => new MonthlyEmissions
                {
                    MonthName = new DateTime(today.Year, g.Key, 1).ToString("MMMM"),
                    TotalCarbonKg = g.Sum(e => e.CarbonKg)
                })
                .OrderBy(m => DateTime.ParseExact(m.MonthName, "MMMM", null).Month)
                .ToList();
        }
    }
}