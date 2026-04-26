using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PLUTO_Carbon_Footprint_Tracker.Data;
using PLUTO_Carbon_Footprint_Tracker.ViewModels;

namespace PLUTO_Carbon_Footprint_Tracker.Pages.Admin.AdminEmission
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly CFTDBContext _context;

        public IndexModel(CFTDBContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public IList<EmissionSearchVM> ActivityItems { get; set; } = new List<EmissionSearchVM>();

        public async Task<IActionResult> OnGetAsync(string? searchTerm)
        {
            SearchTerm = searchTerm;

            var query = _context.Activities.Include(a => a.ActivityType).AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var term = SearchTerm.Trim().ToLower();

                query = query.Where(a => a.Name.ToLower().Contains(term) ||
                a.Unit.ToLower().Contains(term) || a.ActivityType.Name.ToLower().Contains(term));
            }

            ActivityItems = await query.OrderBy(a => a.ActivityType.Name).ThenBy(a => a.Name)
                .Select(a => new EmissionSearchVM
                {
                    Id = a.Id,
                    ActivityName = a.Name,
                    ActivityTypeName = a.ActivityType.Name,
                    Unit = a.Unit,
                    EmissionFactor = a.EmissionFactor,
                    IsActive = a.IsActive
                })
                .ToListAsync();

            return Page();
        }
    }
}