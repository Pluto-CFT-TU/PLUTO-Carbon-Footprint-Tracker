using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PLUTO_Carbon_Footprint_Tracker.Data;

namespace PLUTO_Carbon_Footprint_Tracker.Pages.NewsPage
{
    public class AllNewsModel : PageModel
    {
        private readonly CFTDBContext _context;

        public AllNewsModel(CFTDBContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public IList<NewsArticle> NewsItems { get; set; } = new List<NewsArticle>();

        public async Task<IActionResult> OnGetAsync(string? searchTerm)
        {
            SearchTerm = searchTerm;

            var query = _context.NewsArticles
                .Where(n => n.IsPublished)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var term = SearchTerm.Trim().ToLower();

                query = query.Where(n =>
                    n.Title.ToLower().Contains(term) ||
                    n.Summary.ToLower().Contains(term) ||
                    n.Content.ToLower().Contains(term));
            }

            NewsItems = await query
                .OrderByDescending(n => n.PublishedAt)
                .ToListAsync();

            return Page();
        }
    }
}