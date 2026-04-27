using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PLUTO_Carbon_Footprint_Tracker.Data;

namespace PLUTO_Carbon_Footprint_Tracker.Pages.Admin.AdminTips
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
        public Tips Tips { get; set; } = default!;

        public List<SelectListItem> ActivityTypeItems { get; set; } = [];

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            // Fetch the tip from the database
            var tips = await _context.Tips.FirstOrDefaultAsync(m => m.Id == id);

            if (tips == null)
            {
                return NotFound();
            }
            // Load the tip data into the page model
            Tips = tips;
            // Load activity types for the dropdown
            ActivityTypeItems = await _context.ActivityTypes.OrderBy(at => at.Name)
                .Select(at => new SelectListItem { Value = at.Id.ToString(), Text = at.Name })
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Remove validation for ActivityType since it's a dropdown and will always have
            // a value (even if it's an empty string)
            ModelState.Remove("Tips.ActivityType");
            // Validate required fields
            if (!ModelState.IsValid)
            {
                // If validation fails, reload the activity types for the dropdown
                // and return to the page
                ActivityTypeItems = await _context.ActivityTypes.OrderBy(at => at.Name)
                    .Select(at => new SelectListItem
                    {
                        Value = at.Id.ToString(),
                        Text = at.Name
                    }).ToListAsync();

                return Page();
            }

            _context.Attach(Tips).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TipsExists(Tips.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToPage("./Index");
        }

        private bool TipsExists(int id)
        {
            return _context.Tips.Any(e => e.Id == id);
        }
    }
}