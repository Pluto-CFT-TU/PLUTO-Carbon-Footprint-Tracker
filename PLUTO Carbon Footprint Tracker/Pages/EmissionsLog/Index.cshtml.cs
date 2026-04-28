using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PLUTO_Carbon_Footprint_Tracker.Data;
using PLUTO_Carbon_Footprint_Tracker.Services;

namespace PLUTO_Carbon_Footprint_Tracker.Pages.EmissionsLog
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

        public DateTime SelectedDate { get; set; }
        public decimal TotalCarbonKg { get; set; }
        public decimal AveragePersonDailyCarbonKg { get; set; } = 26m;
        public decimal DifferenceFromAverageKg { get; set; }
        public bool IsBelowAverage { get; set; }
        public Dictionary<string, List<EmissionEntry>> GroupedEntries { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(DateTime? selectedDate)
        {
            // Ensure the user exists in the database and retrieve their information
            var user = await _syncUserDataService.EnsureUserExistsAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Set the selected date to the provided value or default to today's date if not provided
            SelectedDate = (selectedDate ?? DateTime.UtcNow.Date).Date;

            // Retrieve the emission entries for the user on the selected date,
            // including related activity and activity type information
            var entries = await _context.EmissionEntries.Include(e => e.Activity).ThenInclude(a => a.ActivityType)
                .Where(e => e.UserId == user.Id && e.EntryDate == SelectedDate)
                .OrderBy(e => e.Activity.ActivityType.Name)
                .ThenByDescending(e => e.CreatedAt).ToListAsync();

            // Calculate the total carbon emissions for the selected date by summing the CarbonKg values of all entries
            TotalCarbonKg = entries.Sum(e => e.CarbonKg);
            DifferenceFromAverageKg = AveragePersonDailyCarbonKg - TotalCarbonKg;
            IsBelowAverage = TotalCarbonKg < AveragePersonDailyCarbonKg;

            // Group the entries by their activity type and convert the result into a dictionary where the key is
            // the activity type name and the value is a list of emission entries for that activity type
            GroupedEntries = entries.GroupBy(e => e.Activity.ActivityType.Name)
                .ToDictionary(g => g.Key, g => g.ToList());

            return Page();
        }
    }
}