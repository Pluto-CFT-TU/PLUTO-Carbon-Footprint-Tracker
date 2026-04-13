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
    public class IndexModel : PageModel
    {
        private readonly PLUTO_Carbon_Footprint_Tracker.Data.CFTDBContext _context;

        public IndexModel(PLUTO_Carbon_Footprint_Tracker.Data.CFTDBContext context)
        {
            _context = context;
        }

        public IList<EmissionEntry> EmissionEntry { get;set; } = default!;

        public async Task OnGetAsync()
        {
            EmissionEntry = await _context.EmissionEntries
                .Include(e => e.Activity)
                .Include(e => e.User).ToListAsync();
        }
    }
}
