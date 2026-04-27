using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PLUTO_Carbon_Footprint_Tracker.Data;

namespace PLUTO_Carbon_Footprint_Tracker.Pages.Admin.AdminEmission
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly CFTDBContext _context;

        public CreateModel(CFTDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Activity Activity { get; set; } = default!;

        // Select list used to populate the Activity Type dropdown
        public List<SelectListItem> ActivityTypeItems { get; set; } = [];

        public async Task<IActionResult> OnGetAsync()
        {
            // Populate the ActivityTypeItems list with activity types from the database, ordered by name
            ActivityTypeItems = await _context.ActivityTypes.OrderBy(at => at.Name)
                .Select(at => new SelectListItem
                {Value = at.Id.ToString(), Text = at.Name}).ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Remove validation for ActivityType and EmissionEntries since they are not directly bound
            // to the form inputs
            ModelState.Remove("Activity.ActivityType");
            ModelState.Remove("Activity.EmissionEntries");

            // Validate the model state. If it's not valid, repopulate the ActivityTypeItems list and
            // return to the page
            if (!ModelState.IsValid)
            {
                // Repopulate the ActivityTypeItems list to ensure the dropdown is populated when the page is re-rendered
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

            _context.Activities.Add(Activity);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}