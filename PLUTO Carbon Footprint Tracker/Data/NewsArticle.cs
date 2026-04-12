namespace PLUTO_Carbon_Footprint_Tracker.Data
{
    public class NewsArticle
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? SourceName { get; set; }
        public DateTime PublishedAt { get; set; }
        public string? CategoryTag { get; set; }
        public bool IsPublished { get; set; } = true;
    }
}
