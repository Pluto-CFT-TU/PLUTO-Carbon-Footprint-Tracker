namespace PLUTO_Carbon_Footprint_Tracker.Data
{
    public class Tips
    {
        public int Id { get; set; }
        public int? ActivityTypeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? VideoUrl { get; set; }
        public bool IsGeneral { get; set; }
        public bool IsPublished { get; set; } = true;
        public ActivityType? ActivityType { get; set; }
        }
    }
