using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PLUTO_Carbon_Footprint_Tracker.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PLUTO_Carbon_Footprint_Tracker.Pages.Admin.AdminEmission
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly PLUTO_Carbon_Footprint_Tracker.Data.CFTDBContext _context;

        public DeleteModel(PLUTO_Carbon_Footprint_Tracker.Data.CFTDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Activity Activity { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await _context.Activities.Include(a => a.ActivityType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (activity == null)
            {
                return NotFound();
            }
            else
            {
                Activity = activity;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await _context.Activities.FindAsync(id);
            if (activity != null)
            {
                Activity = activity;
                _context.Activities.Remove(Activity);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
