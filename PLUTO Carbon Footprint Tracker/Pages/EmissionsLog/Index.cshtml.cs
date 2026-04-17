using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PLUTO_Carbon_Footprint_Tracker.Data;
using PLUTO_Carbon_Footprint_Tracker.Services;

namespace PLUTO_Carbon_Footprint_Tracker.Pages.EmissionsLog
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

        public DateTime SelectedDate { get; set; }
        public decimal TotalCarbonKg { get; set; }
        public Dictionary<string, List<EmissionEntry>> GroupedEntries { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(DateTime? selectedDate)
        {
            var user = await _syncUserDataService.EnsureUserExistsAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            SelectedDate = (selectedDate ?? DateTime.UtcNow.Date).Date;

            var entries = await _context.EmissionEntries.Include(e => e.Activity)
                .ThenInclude(a => a.ActivityType)
                .Where(e => e.UserId == user.Id && e.EntryDate == SelectedDate)
                .OrderBy(e => e.Activity.ActivityType.Name)
                .ThenByDescending(e => e.CreatedAt).ToListAsync();

            TotalCarbonKg = entries.Sum(e => e.CarbonKg);

            GroupedEntries = entries.GroupBy(e => e.Activity.ActivityType.Name)
                .ToDictionary(g => g.Key, g => g.ToList());

            return Page();
        }
    }
}