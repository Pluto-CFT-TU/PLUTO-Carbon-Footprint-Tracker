using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PLUTO_Carbon_Footprint_Tracker.Data;

namespace PLUTO_Carbon_Footprint_Tracker.Pages.Admin.AdminNews
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly CFTDBContext _context;

        public IndexModel(CFTDBContext context)
        {
            _context = context;
        }

        // Bind the SearchTerm property to the query string for GET requests
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public IList<NewsArticle> NewsItems { get; set; } = new List<NewsArticle>();

        public async Task<IActionResult> OnGetAsync(string? searchTerm)
        {
            // Update the SearchTerm property with the value from the query string
            SearchTerm = searchTerm;
            // Start with the full set of news articles
            var query = _context.NewsArticles.AsQueryable();
            //  If a search term is provided, filter the articles based on the term
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                // Convert the search term to lowercase for case-insensitive comparison
                var term = SearchTerm.Trim().ToLower();
                // Filter the query to include articles where the title, summary,
                // content, or category contains the search term
                query = query.Where(n =>  n.Title.ToLower().Contains(term) ||
                n.Summary.ToLower().Contains(term) ||  n.Content.ToLower().Contains(term) ||
                (n.Category != null && n.Category.ToLower().Contains(term)));
            }
            // Order the results by published date (newest first) and execute the query
            NewsItems = await query.OrderByDescending(n => n.PublishedAt).ToListAsync();

            return Page();
        }
    }
}