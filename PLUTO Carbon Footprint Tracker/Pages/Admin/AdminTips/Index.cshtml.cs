using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PLUTO_Carbon_Footprint_Tracker.Data;
using PLUTO_Carbon_Footprint_Tracker.ViewModels;

namespace PLUTO_Carbon_Footprint_Tracker.Pages.Admin.AdminTips
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

        public IList<TipSearchVM> TipItems { get; set; } = new List<TipSearchVM>();

        public async Task<IActionResult> OnGetAsync(string? searchTerm)
        {
            // Update the SearchTerm property with the value from the query string
            SearchTerm = searchTerm;

            // Start with the full set of tips, including their related ActivityType
            var query = _context.Tips.Include(t => t.ActivityType).AsQueryable();

            // If a search term is provided, filter the tips based on the title,
            // content, or activity type name
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                // Convert the search term to lowercase for case-insensitive comparison
                var term = SearchTerm.Trim().ToLower();
                // Filter the tips based on the search term
                query = query.Where(t => t.Title.ToLower().Contains(term) ||
                t.Content.ToLower().Contains(term) ||
                (t.ActivityType != null && t.ActivityType.Name.ToLower().Contains(term)));
            }

            // Order the tips by IsFaq and then by Title, and project them into the TipSearchVM
            TipItems = await query.OrderBy(t => t.IsFaq).ThenBy(t => t.Title)
                .Select(t => new TipSearchVM
                {
                    Id = t.Id,
                    Title = t.Title,
                    ActivityTypeName = t.ActivityType != null ? t.ActivityType.Name : null,
                    IsFaq = t.IsFaq
                }).ToListAsync();

            return Page();
        }
    }
}