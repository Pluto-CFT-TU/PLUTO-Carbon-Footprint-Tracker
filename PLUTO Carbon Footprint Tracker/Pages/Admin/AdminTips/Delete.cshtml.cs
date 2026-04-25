using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PLUTO_Carbon_Footprint_Tracker.Data;

namespace PLUTO_Carbon_Footprint_Tracker.Pages.Admin.AdminTips
{
    public class DeleteModel : PageModel
    {
        private readonly PLUTO_Carbon_Footprint_Tracker.Data.CFTDBContext _context;

        public DeleteModel(PLUTO_Carbon_Footprint_Tracker.Data.CFTDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Tips Tips { get; set; } = default!;

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
            else
            {
                Tips = tips;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tips = await _context.Tips.FindAsync(id);
            if (tips != null)
            {
                Tips = tips;
                _context.Tips.Remove(Tips);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
