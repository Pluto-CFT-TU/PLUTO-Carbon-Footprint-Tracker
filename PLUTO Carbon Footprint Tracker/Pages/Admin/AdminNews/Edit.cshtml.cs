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
            LoadImageItems();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                LoadImageItems();
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
        private void LoadImageItems()
        {
            // Predefined list of image options for the dropdown
            ImageItems = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "-- No image --" },
                new SelectListItem { Value = "/Images/News/Actnow.jpg", Text = "Act Now" },
                new SelectListItem { Value = "/Images/News/Deforest.jpg", Text = "Deforestation" },
                new SelectListItem { Value = "/Images/News/Fire.jpg", Text = "Fire" },
                new SelectListItem { Value = "/Images/News/grassywindmills.jpg", Text = "Windmills" },
                new SelectListItem { Value = "/Images/News/ice.jpg", Text = "Ice Caps" },
                new SelectListItem { Value = "/Images/News/Nofuture.jpg", Text = "No Future" }
            };
        }
    }
}