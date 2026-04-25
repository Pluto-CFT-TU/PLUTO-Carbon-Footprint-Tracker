using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PLUTO_Carbon_Footprint_Tracker.Data;

namespace PLUTO_Carbon_Footprint_Tracker.Pages.Admin.AdminEmission
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly CFTDBContext _context;

        public EditModel(CFTDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Activity Activity { get; set; } = default!;

        public List<SelectListItem> ActivityTypeItems { get; set; } = [];

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await _context.Activities
                .FirstOrDefaultAsync(m => m.Id == id);

            if (activity == null)
            {
                return NotFound();
            }

            Activity = activity;

            ActivityTypeItems = await _context.ActivityTypes
                .OrderBy(at => at.Name)
                .Select(at => new SelectListItem
                {
                    Value = at.Id.ToString(),
                    Text = at.Name
                })
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Activity.ActivityType");
            ModelState.Remove("Activity.EmissionEntries");

            if (!ModelState.IsValid)
            {
                ActivityTypeItems = await _context.ActivityTypes
                    .OrderBy(at => at.Name)
                    .Select(at => new SelectListItem
                    {
                        Value = at.Id.ToString(),
                        Text = at.Name
                    })
                    .ToListAsync();

                return Page();
            }

            _context.Attach(Activity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActivityExists(Activity.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToPage("./Index");
        }

        private bool ActivityExists(int id)
        {
            return _context.Activities.Any(e => e.Id == id);
        }
    }
}