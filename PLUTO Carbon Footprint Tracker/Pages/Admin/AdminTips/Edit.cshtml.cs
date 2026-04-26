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

            var tips = await _context.Tips.FirstOrDefaultAsync(m => m.Id == id);

            if (tips == null)
            {
                return NotFound();
            }

            Tips = tips;

            ActivityTypeItems = await _context.ActivityTypes.OrderBy(at => at.Name)
                .Select(at => new SelectListItem { Value = at.Id.ToString(), Text = at.Name })
                .ToListAsync();

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