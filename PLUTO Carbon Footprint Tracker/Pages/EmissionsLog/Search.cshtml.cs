using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PLUTO_Carbon_Footprint_Tracker.Data;
using PLUTO_Carbon_Footprint_Tracker.ViewModels;

namespace PLUTO_Carbon_Footprint_Tracker.Pages.EmissionsLog
{
    public class SearchModel : PageModel
    {
        private readonly CFTDBContext _context;

        public SearchModel(CFTDBContext context)
        {
            _context = context;
        }
        // Bind the SearchTerm and SelectedDate properties to
        // allow them to be set from query string parameters on GET requests
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }
        // Bind the SelectedDate property to allow it to be set from query
        // string parameters on GET requests
        [BindProperty(SupportsGet = true)]
        public DateTime SelectedDate { get; set; }

        public IList<EmissionSearchVM> ActivityItems { get; set; } = new List<EmissionSearchVM>();

        public async Task<IActionResult> OnGetAsync(string? searchTerm, DateTime? selectedDate)
        {
            // Set default values for search term and selected date
            SearchTerm = searchTerm;
            SelectedDate = (selectedDate ?? DateTime.UtcNow.Date).Date;

            // Build the query to fetch active activities with their types
            var query = _context.Activities.Include(a => a.ActivityType).Where(a => a.IsActive)
                .AsQueryable();

            // Apply search term filter if provided
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var term = SearchTerm.Trim().ToLower();
                // Filter activities where the name or activity type name contains the search term
                query = query.Where(a => a.Name.ToLower().Contains(term) ||
                    a.ActivityType.Name.ToLower().Contains(term));
            }

            // Fetch the filtered and sorted activity items and project them into the EmissionSearchVM
            ActivityItems = await query.OrderBy(a => a.ActivityType.Name).ThenBy(a => a.Name)
                .Select(a => new EmissionSearchVM
                {
                    Id = a.Id,
                    ActivityName = a.Name,
                    ActivityTypeName = a.ActivityType.Name,
                    Unit = a.Unit,
                    EmissionFactor = a.EmissionFactor
                }).ToListAsync();

            return Page();
        }
    }
}