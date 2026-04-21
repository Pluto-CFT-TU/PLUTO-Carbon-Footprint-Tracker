namespace PLUTO_Carbon_Footprint_Tracker.Data
{
    public class Tips
    {
        public int Id { get; set; }
        public int? ActivityTypeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? VideoUrl { get; set; }
        public bool IsFaq { get; set; }
        public ActivityType? ActivityType { get; set; }
        }
    }
