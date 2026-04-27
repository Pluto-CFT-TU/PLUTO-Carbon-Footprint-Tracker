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
            if (principal?.Identity?.IsAuthenticated != true)
            {
                _logger.LogWarning("User is not authenticated.");
                return null;
            }
            var auth0UserId = principal.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ??
                principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(auth0UserId))
            {
                _logger.LogWarning("No Auth0 user ID claim found.");
                foreach (var claim in principal.Claims)
                {
                    _logger.LogInformation("Claim: {Type} = {Value}", claim.Type, claim.Value);
                }

                return null;
            }

            var email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ??
                principal.Claims.FirstOrDefault(c => c.Type == "email")?.Value ??
                string.Empty;

            var username = principal.Claims.FirstOrDefault(c => c.Type == "nickname")?.Value
                ?? principal.Claims.FirstOrDefault(c => c.Type == "name")?.Value
                ?? principal.Claims.FirstOrDefault(c => c.Type == "email")?.Value?.Split('@')[0]
                ?? "User";

            _logger.LogInformation("Auth0UserId: {Auth0UserId}", auth0UserId);
            _logger.LogInformation("Email: {Email}", email);
            _logger.LogInformation("Username: {Username}", username);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Auth0UserId == auth0UserId);

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

            var changes = await _context.SaveChangesAsync();
            _logger.LogInformation("SaveChangesAsync result: {Changes}", changes);
            return user;
        }
    }
}