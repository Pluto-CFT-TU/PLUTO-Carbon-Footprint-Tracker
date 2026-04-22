using Auth0.AspNetCore.Authentication;
using PLUTO_Carbon_Footprint_Tracker.Data;
using PLUTO_Carbon_Footprint_Tracker.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Register database context with the framework - this needs to be added manually
builder.Services.AddDbContext<CFTDBContext>();
builder.Services.AddScoped<SyncUserDataService>();
builder.Services
// -------------------------------------------------------------
// AddAuth0WebAppAuthentication
//
// This registers all authentication services needed to use Auth0
// as the identity provider for a Razor Pages application.
//
// It adds:
//   • Cookie authentication (used by ASP.NET Core)
//   • OpenID Connect handler (used to talk to Auth0)
//   • The "/callback" endpoint used after login
//
// By calling this, ASP.NET Core now knows:
//   - Where to redirect users for login
//   - How to validate tokens returned from Auth0
//   - How to sign users in/out using Auth0
// -------------------------------------------------------------
.AddAuth0WebAppAuthentication(options => {
    // ---------------------------------------------------------
    // Domain
    //
    // This is your Auth0 Tenant URL, such as:
    //     dev-abc123uk.auth0.com
    //
    // The Razor app uses this to:
    //   • Redirect users to Auth0's login page
    //
    // Stored in appsettings.json under "Auth0:Domain"
    // so that secrets are not hardcoded in code.
    // ---------------------------------------------------------
    options.Domain = builder.Configuration["Auth0:Domain"];
    // ---------------------------------------------------------
    // ClientId
    //
    // The unique ID of your application registered in Auth0.
    //
    // Auth0 uses this to:
    //   • Identify WHICH application is trying to authenticate
    //   • Control what callbacks, logouts, and permissions apply
    //
    // Also stored in appsettings.json under "Auth0:ClientId"
    // ---------------------------------------------------------
    options.ClientId = builder.Configuration["Auth0:ClientId"];
    options.Scope = "openid profile email";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// This enables the ASP.NET Core Authentication system.
// It activates all authentication handlers registered in Program.cs,
// including the Auth0 OpenID Connect handler and the cookie handler.
//
//
// WITHOUT this line:
//   → Login will not work
//   → HttpContext.User will always be empty
//   → Protected pages will never detect an authenticated user
// -----------------------------------------------------------------------------
app.UseAuthentication();
// This enables ASP.NET Core's Authorization system.
// It checks whether the authenticated user is allowed to access a resource.
//
// This is required for:
//   • [Authorize] on pages
//   • policy-based authorization
//   • role-based authorization (if used)
//   • conventions like:
//        options.Conventions.AuthorizePage("/Events");
//
// WITHOUT this line:
//   → Users can log in, but authorization rules won't run
//   → All pages behave as if they are public
// -----------------------------------------------------------------------------
app.UseAuthorization();

app.MapRazorPages();

app.Run();
