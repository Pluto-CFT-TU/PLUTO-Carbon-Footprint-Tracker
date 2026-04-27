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

        // BindProperty with SupportsGet = true allows the SearchTerm to be bound
        // from query string parameters on GET requests
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public IList<EmissionSearchVM> ActivityItems { get; set; } = new List<EmissionSearchVM>();

        public async Task<IActionResult> OnGetAsync(string? searchTerm)
        {
            // Update the SearchTerm property with the value from the query string
            SearchTerm = searchTerm;
            // Start with a query that includes the related ActivityType data
            var query = _context.Activities.Include(a => a.ActivityType).AsQueryable();

            // If a search term is provided, filter the query to include only activities that match the search term
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                // Convert the search term to lowercase for case-insensitive comparison
                var term = SearchTerm.Trim().ToLower();
                // Filter the query to include activities where the name, unit, or activity type name contains the search term
                query = query.Where(a => a.Name.ToLower().Contains(term) ||
                a.Unit.ToLower().Contains(term) || a.ActivityType.Name.ToLower().Contains(term));
            }

            // Order the results by activity type name and then by activity name, and project
            // the results into the EmissionSearchVM view model
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