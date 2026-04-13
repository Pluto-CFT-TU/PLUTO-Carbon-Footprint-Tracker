using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PLUTO_Carbon_Footprint_Tracker.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public async Task OnGet()
        {
            // Build the logout authentication properties
            // Log the user out of Auth0 (the identity provider)
            // Log the user out of the local application (cookie)
            var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
                .WithRedirectUri("/")
                .Build();

            await HttpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        }
    }
}
