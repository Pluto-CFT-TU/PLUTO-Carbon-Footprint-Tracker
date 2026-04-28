using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PLUTO_Carbon_Footprint_Tracker.Data;

namespace PLUTO_Carbon_Footprint_Tracker.Pages.NewsPage
{
    [Authorize]
    public class AllNewsModel : PageModel
    {
        private readonly CFTDBContext _context;

        public AllNewsModel(CFTDBContext context)
        {
            _context = context;
        }
        // Bind the SearchTerm property to allow it to be set from query string parameters on GET requests
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public IList<NewsArticle> NewsItems { get; set; } = new List<NewsArticle>();

        public async Task<IActionResult> OnGetAsync(string? searchTerm)
        {
            // Update the SearchTerm property with the value from the query string
            SearchTerm = searchTerm;

            // Start with a base query for published news articles
            var query = _context.NewsArticles
                .Where(n => n.IsPublished)
                .AsQueryable();

            // If a search term is provided, filter the news articles based on the term
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                // Convert the search term to lowercase for case-insensitive searching
                var term = SearchTerm.Trim().ToLower();
                // Filter the news articles where the title, summary, or content contains the search term
                query = query.Where(n =>
                    n.Title.ToLower().Contains(term) ||
                    n.Summary.ToLower().Contains(term) ||
                    n.Content.ToLower().Contains(term));
            }
            // Order the results by published date in descending order and execute the query
            NewsItems = await query.OrderByDescending(n => n.PublishedAt).ToListAsync();

            return Page();
        }
    }
}