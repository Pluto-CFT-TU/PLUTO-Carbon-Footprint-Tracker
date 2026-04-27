using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PLUTO_Carbon_Footprint_Tracker.Data;
using PLUTO_Carbon_Footprint_Tracker.ViewModels;

namespace PLUTO_Carbon_Footprint_Tracker.Pages.TipsPage
{
    public class AllFaqsModel : PageModel
    {
        private readonly CFTDBContext _context;

        public AllFaqsModel(CFTDBContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public IList<TipSearchVM> FaqItems { get; set; } = new List<TipSearchVM>();

        public async Task<IActionResult> OnGetAsync(string? searchTerm)
        {
            // Set the SearchTerm property to the value from the query string
            SearchTerm = searchTerm;
            // Start with a query that selects all tips marked as FAQs
            var query = _context.Tips.Where(t => t.IsFaq).AsQueryable();

            // If a search term is provided, filter the query to include only
            // FAQs where the title or content contains the search term
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                // Convert the search term to lowercase for case-insensitive comparison
                var term = SearchTerm.Trim().ToLower();
                // Filter the query to include only FAQs where the title or content contains the search term
                query = query.Where(t => t.Title.ToLower().Contains(term) ||
                t.Content.ToLower().Contains(term));
            }

            // Execute the query and project the results into a list of TipSearchVM objects
            FaqItems = await query.OrderBy(t => t.Title)
                .Select(t => new TipSearchVM
                {
                    Id = t.Id,
                    Title = t.Title,
                    ActivityTypeName = null,
                    IsFaq = t.IsFaq
                }).ToListAsync();

            return Page();
        }
    }
}