namespace PLUTO_Carbon_Footprint_Tracker.Data
{
    public class Activity
    {
        public int Id { get; set; }
        public int ActivityTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal EmissionFactor { get; set; }
        public bool IsActive { get; set; } = true;

        public ActivityType ActivityType { get; set; } = null!;
        public ICollection<EmissionEntry> EmissionEntries { get; set; } = new List<EmissionEntry>();
    }
}
