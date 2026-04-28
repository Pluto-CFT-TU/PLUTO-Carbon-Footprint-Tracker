using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PLUTO_Carbon_Footprint_Tracker.Data;
using System.Threading.Tasks;

namespace PLUTO_Carbon_Footprint_Tracker.Pages.Admin.AdminNews
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly CFTDBContext _context;

        public CreateModel(CFTDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public NewsArticle NewsArticle { get; set; } = default!;
        public List<string> ImageOptions { get; set; }

        public List<SelectListItem> ImageItems { get; set; } = [];

        public async Task<IActionResult> OnGet()
        {
            LoadImageItemsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validate required fields
            if (!ModelState.IsValid)
            {
                LoadImageItemsAsync();
                return Page();
            }

            // Auto set publish date
            NewsArticle.PublishedAt = DateTime.UtcNow;

            // Save the new article to the database
            _context.NewsArticles.Add(NewsArticle);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
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