namespace PLUTO_Carbon_Footprint_Tracker.ViewModels
{
    public class TipSearchVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ActivityTypeName { get; set; }
        public bool IsFaq { get; set; }
    }
}