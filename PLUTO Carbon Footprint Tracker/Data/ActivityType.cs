namespace PLUTO_Carbon_Footprint_Tracker.Data
{
    public class ActivityType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<Activity> Activities { get; set; } = new List<Activity>();
    }
}
