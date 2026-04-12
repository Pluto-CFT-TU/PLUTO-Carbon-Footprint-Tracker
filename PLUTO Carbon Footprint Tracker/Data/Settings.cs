namespace PLUTO_Carbon_Footprint_Tracker.Data
{
    public class Settings
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FontSize { get; set; } = 16;
        public bool HighContrast { get; set; }
        public bool TextToSpeech { get; set; }
        public string? ColourBlindMode { get; set; }
        public bool AllowAnalytics { get; set; }
        public bool AllowMarketingEmails { get; set; }
        public User User { get; set; } = null!;
        }
    }
