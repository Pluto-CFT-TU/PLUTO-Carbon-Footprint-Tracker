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
            var user = await _syncUserDataService.EnsureUserExistsAsync(User);
            if (user == null)
            {
                return;
            }

            CurrentUser = user;

            var today = DateTime.UtcNow.Date;

            var userEntries = await _context.EmissionEntries
                .Where(e => e.UserId == user.Id).Include(e => e.Activity)
                .ThenInclude(a => a.ActivityType).ToListAsync();

            TotalEntriesLogged = userEntries.Count;
            TotalCarbonLogged = userEntries.Sum(e => e.CarbonKg);

            var highestCarbonActivity = userEntries
                .GroupBy(e => e.Activity.ActivityType.Name)
                .Select(g => new
                {
                    ActivityTypeName = g.Key,
                    TotalCarbon = g.Sum(e => e.CarbonKg)
                })
                .OrderByDescending(g => g.TotalCarbon).FirstOrDefault();

            if (highestCarbonActivity != null)
            {
                HighestCarbonActivityType = highestCarbonActivity.ActivityTypeName;
            }

            WeeklyTotal = userEntries.Where(e => e.EntryDate.Date >= today.AddDays(-6))
                .Sum(e => e.CarbonKg);

            MonthlyTotal = userEntries.Where(e => e.EntryDate.Month == today.Month &&
            e.EntryDate.Year == today.Year).Sum(e => e.CarbonKg);

            YearTotal = userEntries.Where(e => e.EntryDate.Year == today.Year)
                .Sum(e => e.CarbonKg);

            var bestDay = userEntries.GroupBy(e => e.EntryDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalCarbon = g.Sum(e => e.CarbonKg)
                })
                .OrderBy(g => g.TotalCarbon).FirstOrDefault();

            if (bestDay != null)
            {
                BestDayDate = bestDay.Date;
                BestDayCarbon = bestDay.TotalCarbon;
            }
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