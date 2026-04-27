using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PLUTO_Carbon_Footprint_Tracker.Data;

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

        public List<SelectListItem> ImageItems { get; set; } = [];

        public IActionResult OnGet()
        {
            LoadImageItems();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validate required fields
            if (!ModelState.IsValid)
            {
                LoadImageItems();
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