using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PLUTO_Carbon_Footprint_Tracker.Data;
using PLUTO_Carbon_Footprint_Tracker.Services;

namespace PLUTO_Carbon_Footprint_Tracker.Pages.EmissionsLog
{
    public class CreateModel : PageModel
    {
        private readonly CFTDBContext _context;
        private readonly SyncUserDataService _syncUserDataService;

        public CreateModel(CFTDBContext context, SyncUserDataService syncUserDataService)
        {
            _context = context;
            _syncUserDataService = syncUserDataService;
        }

        public Activity? Activity { get; set; }

        [BindProperty]
        public int ActivityId { get; set; }

        [BindProperty]
        public decimal Quantity { get; set; }

        [BindProperty]
        public DateTime SelectedDate { get; set; }

        public decimal CarbonKg { get; set; }

        public async Task<IActionResult> OnGetAsync(int? activityId, DateTime? selectedDate)
        {
            if (activityId == null)
            {
                return NotFound();
            }

            Activity = await _context.Activities.Include(a => a.ActivityType)
                .FirstOrDefaultAsync(a => a.Id == activityId && a.IsActive);

            if (Activity == null)
            {
                return NotFound();
            }

            ActivityId = Activity.Id;
            SelectedDate = (selectedDate ?? DateTime.UtcNow.Date).Date;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Activity = await _context.Activities.Include(a => a.ActivityType)
                .FirstOrDefaultAsync(a => a.Id == ActivityId && a.IsActive);

            if (Activity == null)
            {
                return NotFound();
            }

            if (Quantity <= 0)
            {
                ModelState.AddModelError(nameof(Quantity), "Quantity must be greater than 0.");
                return Page();
            }

            var user = await _syncUserDataService.EnsureUserExistsAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            CarbonKg = Quantity * Activity.EmissionFactor;

            var entry = new EmissionEntry
            {
                UserId = user.Id,
                ActivityId = Activity.Id,
                Quantity = Quantity,
                CarbonKg = CarbonKg,
                EntryDate = SelectedDate.Date,
                CreatedAt = DateTime.UtcNow
            };

            _context.EmissionEntries.Add(entry);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index", new { selectedDate = SelectedDate.ToString("yyyy-MM-dd") });
        }
    }
}