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
            SearchTerm = searchTerm;

            var query = _context.Tips.Where(t => t.IsFaq).AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var term = SearchTerm.Trim().ToLower();

                query = query.Where(t => t.Title.ToLower().Contains(term) ||
                t.Content.ToLower().Contains(term));
            }

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