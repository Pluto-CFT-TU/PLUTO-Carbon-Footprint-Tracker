namespace PLUTO_Carbon_Footprint_Tracker.Data
{
    public class User
    {
        public int Id { get; set; }
        public string Auth0UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string? Membership { get; set; }
        public string? Rank { get; set; }
    }
}
