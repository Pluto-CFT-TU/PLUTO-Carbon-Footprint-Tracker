using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PLUTO_Carbon_Footprint_Tracker.Services;

namespace PLUTO_Carbon_Footprint_Tracker.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly SyncUserDataService _syncUserDataService;

        public IndexModel(
            ILogger<IndexModel> logger,
            SyncUserDataService syncUserDataService)
        {
            _logger = logger;
            _syncUserDataService = syncUserDataService;
        }
        public async Task OnGetAsync()
        {
            await _syncUserDataService.EnsureUserExistsAsync(User);
        }
    }
}