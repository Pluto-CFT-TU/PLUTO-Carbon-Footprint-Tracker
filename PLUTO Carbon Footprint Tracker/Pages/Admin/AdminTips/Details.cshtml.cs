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
    public class DetailsModel : PageModel
    {
        private readonly PLUTO_Carbon_Footprint_Tracker.Data.CFTDBContext _context;

        public DetailsModel(PLUTO_Carbon_Footprint_Tracker.Data.CFTDBContext context)
        {
            _context = context;
        }

        public Tips Tips { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tips = await _context.Tips.Include(t => t.ActivityType)
                .FirstOrDefaultAsync(m => m.Id == id);

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
    }
}
