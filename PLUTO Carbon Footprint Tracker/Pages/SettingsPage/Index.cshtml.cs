using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PLUTO_Carbon_Footprint_Tracker.Data;
using PLUTO_Carbon_Footprint_Tracker.Services;

namespace PLUTO_Carbon_Footprint_Tracker.Pages.SettingsPage
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly CFTDBContext _context;
        private readonly SyncUserDataService _syncUserDataService;

        public IndexModel(CFTDBContext context, SyncUserDataService syncUserDataService)
        {
            _context = context;
            _syncUserDataService = syncUserDataService;
        }

        [BindProperty]
        public Settings Settings { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            // Ensure the user exists in the database and retrieve their settings
            var user = await _syncUserDataService.EnsureUserExistsAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            // Try to find existing settings for the user
            var settings = await _context.Settings.FirstOrDefaultAsync(s => s.UserId == user.Id);

            // If no settings exist, create default settings for the user
            if (settings == null)
            {
                settings = new Settings
                {
                    UserId = user.Id,
                    FontSize = 16,
                    HighContrast = false,
                    TextToSpeech = false,
                    ColourBlindMode = "None",
                    AllowAnalytics = false,
                    AllowMarketingEmails = false
                };

                _context.Settings.Add(settings);
                await _context.SaveChangesAsync();
            }
            // Bind the settings to the page model for display
            Settings = settings;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Ensure the user exists in the database
            var user = await _syncUserDataService.EnsureUserExistsAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            // Try to find existing settings for the user
            var existingSettings = await _context.Settings.FirstOrDefaultAsync(s => s.UserId == user.Id);
            // If no settings exist, create new settings for the user
            if (existingSettings == null)
            {
                existingSettings = new Settings
                {
                    UserId = user.Id
                };

                _context.Settings.Add(existingSettings);
            }
            // Update the existing settings with the values from the form
            existingSettings.FontSize = Settings.FontSize;
            existingSettings.HighContrast = Settings.HighContrast;
            existingSettings.TextToSpeech = Settings.TextToSpeech;
            existingSettings.ColourBlindMode = Settings.ColourBlindMode;
            existingSettings.AllowAnalytics = Settings.AllowAnalytics;
            existingSettings.AllowMarketingEmails = Settings.AllowMarketingEmails;
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}