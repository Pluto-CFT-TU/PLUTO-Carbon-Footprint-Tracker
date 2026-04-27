using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PLUTO_Carbon_Footprint_Tracker.Data;
using PLUTO_Carbon_Footprint_Tracker.Services;

namespace PLUTO_Carbon_Footprint_Tracker.Pages.EmissionsLog
{
    public class EditModel : PageModel
    {
        private readonly CFTDBContext _context;
        private readonly SyncUserDataService _syncUserDataService;

        public EditModel(CFTDBContext context, SyncUserDataService syncUserDataService)
        {
            _context = context;
            _syncUserDataService = syncUserDataService;
        }

        [BindProperty]
        public EmissionEntry EmissionEntry { get; set; } = default!;
        public string ActivityName { get; set; } = string.Empty;
        public string ActivityUnit { get; set; } = string.Empty;
        public decimal EmissionFactor { get; set; }
        public decimal EmissionAmount { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            // Ensures the user exists and owns the entry before we can display the page
            var user = await _syncUserDataService.EnsureUserExistsAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            // Loads the entry with the related activity to display the details on the page
            var emissionEntry = await _context.EmissionEntries.Include(e => e.Activity)
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == user.Id);

            if (emissionEntry == null)
            {
                return NotFound();
            }

            // Populates the page model properties with the entry and related activity details
            EmissionEntry = emissionEntry;
            ActivityName = emissionEntry.Activity.Name;
            ActivityUnit = emissionEntry.Activity.Unit;
            EmissionFactor = emissionEntry.Activity.EmissionFactor;
            EmissionAmount = emissionEntry.CarbonKg;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validates the user exists and owns the entry before allowing updates to be made
            var user = await _syncUserDataService.EnsureUserExistsAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Loads the existing entry with the related activity to ensure it exists and
            // to allow updates to be made to the correct entry
            var existingEntry = await _context.EmissionEntries.Include(e => e.Activity)
                .FirstOrDefaultAsync(e => e.Id == EmissionEntry.Id && e.UserId == user.Id);

            if (existingEntry == null)
            {
                return NotFound();
            }

            // Prevents errors being presented for properties not being posted by the form
            ModelState.Remove("EmissionEntry.User");
            ModelState.Remove("EmissionEntry.Activity");

            // Validates the form data and if invalid, re-populates the page model properties
            // with the existing entry details
            if (!ModelState.IsValid)
            {
                ActivityName = existingEntry.Activity.Name;
                ActivityUnit = existingEntry.Activity.Unit;
                EmissionFactor = existingEntry.Activity.EmissionFactor;
                EmissionAmount = existingEntry.CarbonKg;
                return Page();
            }

            // Updates the existing entry with the new quantity and recalculates the carbon 
            // amount based on the activity's emission factor
            existingEntry.Quantity = EmissionEntry.Quantity;
            existingEntry.CarbonKg = EmissionEntry.Quantity * existingEntry.Activity.EmissionFactor;

            await _context.SaveChangesAsync();

            // Redirects back to the index page with the selected date set to the entry date of the updated entry
            return RedirectToPage("./Index", new
            {
                selectedDate = existingEntry.EntryDate.ToString("yyyy-MM-dd")
            });
        }
    }
}