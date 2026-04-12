namespace PLUTO_Carbon_Footprint_Tracker.Data
{
    public class NotificationSetting
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public bool EmailNotificationsEnabled { get; set; }
        public bool SmsNotificationsEnabled { get; set; }
        public bool PushNotificationsEnabled { get; set; }
        public bool Muted { get; set; }

        public User User { get; set; } = null!;
    }
}
