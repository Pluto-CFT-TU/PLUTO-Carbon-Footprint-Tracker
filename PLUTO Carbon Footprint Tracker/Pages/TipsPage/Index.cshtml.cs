using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PLUTO_Carbon_Footprint_Tracker.Data;
using PLUTO_Carbon_Footprint_Tracker.Services;

namespace PLUTO_Carbon_Footprint_Tracker.Pages.TipsPage
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

        public IList<Tips> RecommendedTips { get; set; } = new List<Tips>();
        public IList<Tips> GeneralTips { get; set; } = new List<Tips>();

        public string? HighestCarbonActivityType { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _syncUserDataService.EnsureUserExistsAsync(User);
            if (user == null)
            {
                return;
            }

            // Load the user's entries first
            var userEntries = await _context.EmissionEntries
                .Where(e => e.UserId == user.Id)
                .Include(e => e.Activity)
                .ThenInclude(a => a.ActivityType)
                .ToListAsync();

            // Find the activity type with the highest total carbon
            var highestCarbonActivity = userEntries
                .GroupBy(e => e.Activity.ActivityType.Name)
                .Select(g => new
                {
                    ActivityTypeName = g.Key,
                    TotalCarbon = g.Sum(e => e.CarbonKg)
                })
                .OrderByDescending(g => g.TotalCarbon)
                .FirstOrDefault();

            List<int> recommendedIds = new();

            if (highestCarbonActivity != null)
            {
                HighestCarbonActivityType = highestCarbonActivity.ActivityTypeName;

                RecommendedTips = await _context.Tips
                    .Include(t => t.ActivityType)
                    .Where(t => t.ActivityType != null &&
                                t.ActivityType.Name == HighestCarbonActivityType)
                    .OrderBy(t => t.Title)
                    .Take(5)
                    .ToListAsync();

                recommendedIds = RecommendedTips.Select(t => t.Id).ToList();
            }

            // Load more tips not already shown
            GeneralTips = await _context.Tips
                .Include(t => t.ActivityType)
                .Where(t => !recommendedIds.Contains(t.Id))
                .OrderBy(t => t.Title)
                .Take(4)
                .ToListAsync();
        }
    }
}