namespace PLUTO_Carbon_Footprint_Tracker.ViewModels
{
    public class EmissionSearchVM
    {
        public int Id { get; set; }
        public string ActivityName { get; set; } = string.Empty;
        public string ActivityTypeName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal EmissionFactor { get; set; }
        public bool IsActive { get; set; }
    }
}
