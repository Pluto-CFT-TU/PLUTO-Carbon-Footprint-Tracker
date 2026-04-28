using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PLUTO_Carbon_Footprint_Tracker.Data;
using PLUTO_Carbon_Footprint_Tracker.Services;

namespace PLUTO_Carbon_Footprint_Tracker.Pages.EmissionsLog
{
    [Authorize]
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

            // Load the activity with its type to display relevant information on the page
            Activity = await _context.Activities.Include(a => a.ActivityType)
                .FirstOrDefaultAsync(a => a.Id == activityId && a.IsActive);

            if (Activity == null)
            {
                return NotFound();
            }
            // Set the ActivityId for the form and default the selected date to today if not provided
            ActivityId = Activity.Id;
            SelectedDate = (selectedDate ?? DateTime.UtcNow.Date).Date;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validate the activity exists and is active
            Activity = await _context.Activities.Include(a => a.ActivityType)
                .FirstOrDefaultAsync(a => a.Id == ActivityId && a.IsActive);

            if (Activity == null)
            {
                return NotFound();
            }
            // Validate the quantity the user is trying to log is greater than 0
            if (Quantity <= 0)
            {
                ModelState.AddModelError(nameof(Quantity), "Quantity must be greater than 0.");
                return Page();
            }

            // Ensure the user exists in the database, if not create a new user record based on their Auth0 information
            var user = await _syncUserDataService.EnsureUserExistsAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Calculate the carbon emissions based on the quantity and the activity's emission factor
            CarbonKg = Quantity * Activity.EmissionFactor;

            // Create a new emission entry record and save it to the database
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

            // After saving it redirects back to the Index page of the Emission Log and uses the date of the saved entry
            // to redirect back to the correct date in the log so the user can continue viewing their entries from that date
            // without having to manually navigate back to it
            return RedirectToPage("./Index", new { selectedDate = SelectedDate.ToString("yyyy-MM-dd") });
        }
    }
}