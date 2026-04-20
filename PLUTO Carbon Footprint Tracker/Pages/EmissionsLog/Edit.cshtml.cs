using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PLUTO_Carbon_Footprint_Tracker.Data;
using PLUTO_Carbon_Footprint_Tracker.Services;

namespace PLUTO_Carbon_Footprint_Tracker.Pages.EmissionsLog
{
    [Authorize]
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

            var user = await _syncUserDataService.EnsureUserExistsAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var emissionEntry = await _context.EmissionEntries.Include(e => e.Activity)
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == user.Id);

            if (emissionEntry == null)
            {
                return NotFound();
            }

            EmissionEntry = emissionEntry;
            ActivityName = emissionEntry.Activity.Name;
            ActivityUnit = emissionEntry.Activity.Unit;
            EmissionFactor = emissionEntry.Activity.EmissionFactor;
            EmissionAmount = emissionEntry.CarbonKg;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _syncUserDataService.EnsureUserExistsAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var existingEntry = await _context.EmissionEntries.Include(e => e.Activity)
                .FirstOrDefaultAsync(e => e.Id == EmissionEntry.Id && e.UserId == user.Id);

            if (existingEntry == null)
            {
                return NotFound();
            }

            // Prevents errors being presented for properties not being posted by the form
            ModelState.Remove("EmissionEntry.User");
            ModelState.Remove("EmissionEntry.Activity");

            if (!ModelState.IsValid)
            {
                ActivityName = existingEntry.Activity.Name;
                ActivityUnit = existingEntry.Activity.Unit;
                EmissionFactor = existingEntry.Activity.EmissionFactor;
                EmissionAmount = existingEntry.CarbonKg;
                return Page();
            }

            existingEntry.Quantity = EmissionEntry.Quantity;
            existingEntry.CarbonKg = EmissionEntry.Quantity * existingEntry.Activity.EmissionFactor;

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index", new
            {
                selectedDate = existingEntry.EntryDate.ToString("yyyy-MM-dd")
            });
        }
    }
}