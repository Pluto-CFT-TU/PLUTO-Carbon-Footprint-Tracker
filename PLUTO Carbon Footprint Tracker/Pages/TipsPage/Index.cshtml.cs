using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PLUTO_Carbon_Footprint_Tracker.Data;
using PLUTO_Carbon_Footprint_Tracker.Services;

namespace PLUTO_Carbon_Footprint_Tracker.Pages.TipsPage
{
    [Authorize]
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
        public IList<Tips> FaqItems { get; set; } = new List<Tips>();
        public string? HighestCarbonActivityType { get; set; }

        public async Task OnGetAsync()
        {
            // Ensure the user exists in our database and load their data
            var user = await _syncUserDataService.EnsureUserExistsAsync(User);
            if (user == null)
            {
                return ;
            }

            // Load the user's emission entries along with their activities and activity types
            var userEntries = await _context.EmissionEntries.Where(e => e.UserId == user.Id)
                .Include(e => e.Activity).ThenInclude(a => a.ActivityType).ToListAsync();

            // Group entries by activity type and calculate total carbon for each type
            var highestCarbonActivity = userEntries.GroupBy(e => e.Activity.ActivityType.Name)
                .Select(g => new
                {
                    ActivityTypeName = g.Key,
                    TotalCarbon = g.Sum(e => e.CarbonKg)
                })
                .OrderByDescending(g => g.TotalCarbon).FirstOrDefault();

            // Get tips related to the highest carbon activity type
            List<int> recommendedIds = new();

            // If we have a highest carbon activity type, get tips related to it
            if (highestCarbonActivity != null)
            {
                HighestCarbonActivityType = highestCarbonActivity.ActivityTypeName;
                // Get tips related to the highest carbon activity type
                RecommendedTips = await _context.Tips
                    .Include(t => t.ActivityType)
                    .Where(t => !t.IsFaq &&
                                t.ActivityType != null &&
                                t.ActivityType.Name == HighestCarbonActivityType)
                    .OrderBy(t => t.Title)
                    .Take(5)
                    .ToListAsync();
                // Collect the IDs of the recommended tips to exclude them from the general tips
                recommendedIds = RecommendedTips.Select(t => t.Id).ToList();
            }

            // Get general tips that are not FAQs and not already recommended
            GeneralTips = await _context.Tips
                .Include(t => t.ActivityType)
                .Where(t => !t.IsFaq &&
                            !recommendedIds.Contains(t.Id))
                .OrderBy(t => t.Title)
                .Take(4)
                .ToListAsync();
            // Get FAQ items
            FaqItems = await _context.Tips
                .Where(t => t.IsFaq)
                .OrderBy(t => t.Title)
                .Take(4)
                .ToListAsync();
        }
    }
}