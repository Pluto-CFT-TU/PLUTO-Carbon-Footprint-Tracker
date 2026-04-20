using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PLUTO_Carbon_Footprint_Tracker.Data;
using PLUTO_Carbon_Footprint_Tracker.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public IList<Tips> PersonalisedTips { get; set; } = default!;
        public IList<Tips> GeneralTips { get; set; } = default!;
        // USed to descern which activity type the user logs more
        // so more tips related to that activity type can be displayed
        public string? HighestActivityType { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _syncUserDataService.EnsureUserExistsAsync(User);
            if (user == null)
            {
                return;
            }
            var HighestActivity = await _context.EmissionEntries.Where(e => e.UserId == user.Id)
                .Include(e => e.Activity)
                .ThenInclude(a => a.ActivityType)
                .GroupBy(e => e.Activity.ActivityType.Name)
                .Select(g => new { ActivityTypeName = g.Key, TotalCarbon = g.Sum(e => e.CarbonKg) })
                .OrderByDescending(g => g.TotalCarbon)
                .FirstOrDefaultAsync();

            if (HighestActivity != null)
            {
                HighestActivityType = HighestActivity.ActivityTypeName;
                PersonalisedTips = await _context.Tips.Where(t => t.ActivityType.Name == HighestActivityType).ToListAsync();
            }
            GeneralTips = await _context.Tips.Where(t => t.ActivityType == null).ToListAsync();
        }
    }
}
