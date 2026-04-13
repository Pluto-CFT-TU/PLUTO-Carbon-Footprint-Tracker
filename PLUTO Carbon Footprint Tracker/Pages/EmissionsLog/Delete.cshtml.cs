using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PLUTO_Carbon_Footprint_Tracker.Data;

namespace PLUTO_Carbon_Footprint_Tracker.Pages.EmissionsLog
{
    public class DeleteModel : PageModel
    {
        private readonly PLUTO_Carbon_Footprint_Tracker.Data.CFTDBContext _context;

        public DeleteModel(PLUTO_Carbon_Footprint_Tracker.Data.CFTDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public EmissionEntry EmissionEntry { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emissionentry = await _context.EmissionEntries.FirstOrDefaultAsync(m => m.Id == id);

            if (emissionentry == null)
            {
                return NotFound();
            }
            else
            {
                EmissionEntry = emissionentry;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emissionentry = await _context.EmissionEntries.FindAsync(id);
            if (emissionentry != null)
            {
                EmissionEntry = emissionentry;
                _context.EmissionEntries.Remove(EmissionEntry);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
