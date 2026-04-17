namespace PLUTO_Carbon_Footprint_Tracker.Data
{
    public class EmissionEntry
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ActivityId { get; set; }
        public decimal Quantity { get; set; }
        public decimal CarbonKg { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
        public Activity Activity { get; set; } = null!;
    }
}
