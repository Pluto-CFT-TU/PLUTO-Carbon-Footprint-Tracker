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
            ModelState.Remove("Tips.ActivityType");

            if (!ModelState.IsValid)
            {
                ActivityTypeItems = await _context.ActivityTypes.OrderBy(at => at.Name)
                    .Select(at => new SelectListItem
                    {
                        Value = at.Id.ToString(),
                        Text = at.Name
                    }).ToListAsync();

                return Page();
            }

            _context.Tips.Add(Tips);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}