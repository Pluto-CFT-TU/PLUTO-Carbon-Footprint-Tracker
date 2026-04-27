using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PLUTO_Carbon_Footprint_Tracker.Data;

namespace PLUTO_Carbon_Footprint_Tracker.Pages.Admin.AdminTips
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
        public Tips Tips { get; set; } = default!;

        public List<SelectListItem> ActivityTypeItems { get; set; } = [];

        public async Task<IActionResult> OnGetAsync()
        {
            // Load the activity types to populate the dropdown list
            ActivityTypeItems = await _context.ActivityTypes.OrderBy(at => at.Name)
                .Select(at => new SelectListItem
                {
                    Value = at.Id.ToString(),
                    Text = at.Name
                }).ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Remove validation for ActivityType since it's a dropdown and will
            // always have a value (even if it's an empty string)
            ModelState.Remove("Tips.ActivityType");

            // Validate required fields
            if (!ModelState.IsValid)
            {
                // If validation fails, reload the activity types for the dropdown and return to the page
                ActivityTypeItems = await _context.ActivityTypes.OrderBy(at => at.Name)
                    .Select(at => new SelectListItem
                    {
                        Value = at.Id.ToString(),
                        Text = at.Name
                    }).ToListAsync();

                return Page();
            }
            // Save the new tip to the database
            _context.Tips.Add(Tips);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}