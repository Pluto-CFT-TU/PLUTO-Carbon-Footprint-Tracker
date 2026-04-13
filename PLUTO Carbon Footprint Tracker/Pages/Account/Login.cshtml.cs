using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PLUTO_Carbon_Footprint_Tracker.Pages.Account
{
    public class LoginModel : PageModel
    {
        // This method runs when the user navigates to /Account/Login
        // Example: https://localhost:7011/Account/Login
        public async Task OnGet(string returnUrl = "/")
        {
            // -------------------------------------------------------------
            // 1. Build the authentication properties
            // -------------------------------------------------------------
            // LoginAuthenticationPropertiesBuilder is provided by Auth0.
            //
            // We tell it:
            //   - Where Auth0 should redirect the user AFTER successful login
            //
            // This "returnUrl" value usually comes from:
            //   - The page the user originally wanted to visit
            //   - Defaults to "/" if none provided
            // -------------------------------------------------------------
            var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
                        .WithRedirectUri(returnUrl)
                        .Build();

            // -------------------------------------------------------------
            // 2. Trigger the authentication challenge
            // -------------------------------------------------------------
            // This instructs ASP.NET Core to:
            //   - Send the user to Auth0's Hosted Login Page
            //   - Include all required OAuth/OpenID Connect parameters
            //   - Handle the redirect back to /callback automatically
            //  The framework takes care of:
            //   - Redirecting to Auth0
            //   - Processing the response
            //   - Creating the local auth cookie
            // -------------------------------------------------------------
            await HttpContext.ChallengeAsync(
                Auth0Constants.AuthenticationScheme,     // use the Auth0 pipeline
                authenticationProperties                                       // include RedirectUri and state
            );

            // NOTE: This method never returns a page.
            // Once ChallengeAsync is called, ASP.NET immediately redirects
            // the browser to Auth0 — the user never sees this page rendered.
        }
    }
}
