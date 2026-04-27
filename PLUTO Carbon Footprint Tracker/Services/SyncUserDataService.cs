using Microsoft.EntityFrameworkCore;
using PLUTO_Carbon_Footprint_Tracker.Data;
using System.Security.Claims;

namespace PLUTO_Carbon_Footprint_Tracker.Services
{
    public class SyncUserDataService
    {
        private readonly CFTDBContext _context;
        private readonly ILogger<SyncUserDataService> _logger;
        public SyncUserDataService(CFTDBContext context, ILogger<SyncUserDataService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<User?> EnsureUserExistsAsync(ClaimsPrincipal principal)
        {
            // Log all claims for debugging
            if (principal?.Identity?.IsAuthenticated != true)
            {
                _logger.LogWarning("User is not authenticated.");
                return null;
            }
            // Try to get the Auth0 user ID from the claims
            var auth0UserId = principal.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ??
                principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            // If the Auth0 user ID can't be found, log a warning and return null
            if (string.IsNullOrWhiteSpace(auth0UserId))
            {                
                _logger.LogWarning("No Auth0 user ID claim found.");
                foreach (var claim in principal.Claims)
                {
                    _logger.LogInformation("Claim: {Type} = {Value}", claim.Type, claim.Value);
                }

                return null;
            }
            // Retrieve email from the claims
            var email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ??
                principal.Claims.FirstOrDefault(c => c.Type == "email")?.Value ??
                string.Empty;

            var username = principal.Claims.FirstOrDefault(c => c.Type == "nickname")?.Value
                ?? principal.Claims.FirstOrDefault(c => c.Type == "name")?.Value
                ?? principal.Claims.FirstOrDefault(c => c.Type == "email")?.Value?.Split('@')[0]
                ?? "User";

            // Log the extracted values for debugging
            _logger.LogInformation("Auth0UserId: {Auth0UserId}", auth0UserId);
            _logger.LogInformation("Email: {Email}", email);
            _logger.LogInformation("Username: {Username}", username);

            // Check if a user with the given Auth0 user ID already exists in the database
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Auth0UserId == auth0UserId);
            // If the user doesn't exist, create a new one. Otherwise, update the existing user's email and username.
            if (user == null)
            {
                user = new User
                {
                    Auth0UserId = auth0UserId,
                    Email = email,
                    Username = username,
                    Membership = "Free",
                    Rank = "Beginner",
                    CreatedAt = DateTime.UtcNow
                };
                _context.Users.Add(user);
                _logger.LogInformation("Creating new user.");
            }
            else
            {
                user.Email = email;
                user.Username = username;
                user.UpdatedAt = DateTime.UtcNow;
                _logger.LogInformation("Updating existing user.");
            }
            // Save changes to the database and log the result
            var changes = await _context.SaveChangesAsync();
            _logger.LogInformation("SaveChangesAsync result: {Changes}", changes);
            return user;
        }
    }
}