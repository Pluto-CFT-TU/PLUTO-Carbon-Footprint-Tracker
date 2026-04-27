using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PLUTO_Carbon_Footprint_Tracker.Data;

namespace PLUTO_Carbon_Footprint_Tracker.Pages.NewsPage
{
    public class IndexModel : PageModel
    {
        private readonly PLUTO_Carbon_Footprint_Tracker.Data.CFTDBContext _context;

        public IndexModel(PLUTO_Carbon_Footprint_Tracker.Data.CFTDBContext context)
        {
            _context = context;
        }

        public IList<NewsArticle> CurrentNews { get; set; } = new List<NewsArticle>();
        public IList<NewsArticle> MoreNews { get; set; } = new List<NewsArticle>();

        public async Task OnGetAsync()
        {
            // Fetch the 4 most recent published news articles
            CurrentNews = await _context.NewsArticles.Where(n => n.IsPublished)
                .OrderByDescending(n => n.PublishedAt)
                .Take(4)
                .ToListAsync();

            // Get the IDs of the current news articles to exclude them from the "More News" section
            var currentIds = CurrentNews.Select(n => n.Id).ToList();

            // Fetch additional published news articles that are not in the current news list
            var remainingNews = await _context.NewsArticles
                .Where(n => n.IsPublished && !currentIds.Contains(n.Id))
                .ToListAsync();

            // Randomly select 4 news articles from the remaining list for the "More News" section
            MoreNews = remainingNews
                .OrderBy(n => Guid.NewGuid())
                .Take(4)
                .ToList();
        }
    }
}
