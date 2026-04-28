using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PLUTO_Carbon_Footprint_Tracker.Data;
using PLUTO_Carbon_Footprint_Tracker.Services;

namespace PLUTO_Carbon_Footprint_Tracker.Pages.EmissionsLog
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly CFTDBContext _context;
        private readonly SyncUserDataService _syncUserDataService;

        public DeleteModel(CFTDBContext context, SyncUserDataService syncUserDataService)
        {
            _context = context;
            _syncUserDataService = syncUserDataService;
        }

        [BindProperty]
        public EmissionEntry EmissionEntry { get; set; } = default!;

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

            // Includes the Activity and ActivityType to display the details on the page
            var emissionentry = await _context.EmissionEntries.Include(e => e.Activity).ThenInclude(a => a.ActivityType)
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == user.Id);

            if (emissionentry == null)
            {
                return NotFound();
            }
       
            EmissionEntry = emissionentry;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Ensures the user exists and owns the entry before we can delete it
            var user = await _syncUserDataService.EnsureUserExistsAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Loads the entry to ensure it exists and belongs to the user before deletion.
            // We don't need to include the Activity and ActivityType here since we won't be displaying any details after deletion.
            var entry = await _context.EmissionEntries.FirstOrDefaultAsync(e => e.Id == id && e.UserId == user.Id);

            if (entry == null)
            {
                return NotFound();
            }
            // Retrieve the date of the entry before deleting it so it can redirect back to the correct date in the
            // emission log after deletion
            var entryDate = entry.EntryDate.Date;

            // Deletes the entry as it has been confirmed it exists and belongs to the user
            _context.EmissionEntries.Remove(entry);
            await _context.SaveChangesAsync();

            // After deletion it redirects back to the Index page of the Emission Log and uses the date of the deleted entry
            // to redirect back to the correct date in the log so the user can continue viewing their entries from that date
            // without having to manually navigate back to it
            return RedirectToPage("./Index", new { selectedDate = entryDate.ToString("yyyy-MM-dd") });
        }
    }
}
