using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PLUTO_Carbon_Footprint_Tracker.Data;

namespace PLUTO_Carbon_Footprint_Tracker.Pages.Admin.AdminNews
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly CFTDBContext _context;

        public EditModel(CFTDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public NewsArticle NewsArticle { get; set; } = default!;

        public List<string> ImageOptions { get; set; }

        public List<SelectListItem> ImageItems { get; set; } = [];

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve the news article from the database
            var newsArticle = await _context.NewsArticles
                .FirstOrDefaultAsync(m => m.Id == id);

            if (newsArticle == null)
            {
                return NotFound();
            }

            NewsArticle = newsArticle;
            LoadImageItemsAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                LoadImageItemsAsync();
                return Page();
            }

            _context.Attach(NewsArticle).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NewsArticleExists(NewsArticle.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToPage("./Index");
        }

        // Helper method to check if a news article exists in the database
        private bool NewsArticleExists(int id)
        {
            return _context.NewsArticles.Any(e => e.Id == id);
        }

        // Load predefined image options for the dropdown
        private async Task LoadImageItemsAsync()
        {
            ImageOptions = await _context.NewsArticles
                .Where(n => !string.IsNullOrEmpty(n.ImageUrl))
                .Select(n => n.ImageUrl)
                .Distinct()
                .ToListAsync();
            ImageItems = ImageOptions.Select(url => new SelectListItem { Value = url, Text = url }).ToList();
        }
    }
}