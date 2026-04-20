using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PLUTO_Carbon_Footprint_Tracker.Data;
using PLUTO_Carbon_Footprint_Tracker.Services;

namespace PLUTO_Carbon_Footprint_Tracker.Pages.EmissionsLog
{
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

            var user = await _syncUserDataService.EnsureUserExistsAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var emissionentry = await _context.EmissionEntries.Include(e => e.Activity)
                .ThenInclude(a => a.ActivityType)
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

            var user = await _syncUserDataService.EnsureUserExistsAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var entry = await _context.EmissionEntries
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == user.Id);

            if (entry == null)
            {
                return NotFound();
            }

            var entryDate = entry.EntryDate.Date;

            _context.EmissionEntries.Remove(entry);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index", new { selectedDate = entryDate.ToString("yyyy-MM-dd") });
        }
    }
}