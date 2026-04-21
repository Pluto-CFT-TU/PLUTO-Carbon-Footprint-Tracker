using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace PLUTO_Carbon_Footprint_Tracker.Data
{
    public class CFTDBContext : DbContext
    {
        public string DbPath { get; set; }

        public CFTDBContext()
        {
            var folder = Environment.SpecialFolder.MyDocuments;
            var path = Environment.GetFolderPath(folder);
            DbPath = Path.Join(path, "pluto_carbon_footprint_tracker.db");
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<ActivityType> ActivityTypes { get; set; }
        public DbSet<EmissionEntry> EmissionEntries { get; set; }
        public DbSet<NewsArticle> NewsArticles { get; set; }
        public DbSet<NotificationSetting> NotificationSettings { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<Tips> Tips { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite($"Data Source={DbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Activity -> ActivityType (many to one)
            modelBuilder.Entity<Activity>()
                .HasOne(a => a.ActivityType)
                .WithMany(at => at.Activities)
                .HasForeignKey(a => a.ActivityTypeId);

            // EmissionEntry -> User (many to one)
            modelBuilder.Entity<EmissionEntry>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);

            // EmissionEntry -> Activity (many to one)
            modelBuilder.Entity<EmissionEntry>()
                .HasOne(e => e.Activity)
                .WithMany(a => a.EmissionEntries)
                .HasForeignKey(e => e.ActivityId);

            // NotificationSetting -> User (one to one)
            modelBuilder.Entity<NotificationSetting>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId);

            // Settings -> User (one to one)
            modelBuilder.Entity<Settings>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId);

            // Tips -> ActivityType (many to one, optional)
            modelBuilder.Entity<Tips>()
                .HasOne(t => t.ActivityType)
                .WithMany()
                .HasForeignKey(t => t.ActivityTypeId)
                .IsRequired(false);

            modelBuilder.Entity<ActivityType>().HasData(
                new ActivityType { Id = 1, Name = "Transport" },
                new ActivityType { Id = 2, Name = "Food" },
                new ActivityType { Id = 3, Name = "Energy" },
                new ActivityType { Id = 4, Name = "Leisure" }
            );

            modelBuilder.Entity<Activity>().HasData(
                new Activity { Id = 1, Name = "Car Travel", Unit = "miles", EmissionFactor = 0.411m, ActivityTypeId = 1, IsActive = true },
                new Activity { Id = 2, Name = "Bus Travel", Unit = "miles", EmissionFactor = 0.105m, ActivityTypeId = 1, IsActive = true },
                new Activity { Id = 3, Name = "Train Travel", Unit = "miles", EmissionFactor = 0.041m, ActivityTypeId = 1, IsActive = true },
                new Activity { Id = 4, Name = "Flight", Unit = "miles", EmissionFactor = 0.255m, ActivityTypeId = 1, IsActive = true },

                new Activity { Id = 5, Name = "Beef Meal", Unit = "meals", EmissionFactor = 2.500m, ActivityTypeId = 2, IsActive = true },
                new Activity { Id = 6, Name = "Chicken Meal", Unit = "meals", EmissionFactor = 1.100m, ActivityTypeId = 2, IsActive = true },
                new Activity { Id = 7, Name = "Vegetarian Meal", Unit = "meals", EmissionFactor = 0.600m, ActivityTypeId = 2, IsActive = true },
                new Activity { Id = 8, Name = "Dairy Consumption", Unit = "servings", EmissionFactor = 0.300m, ActivityTypeId = 2, IsActive = true },

                new Activity { Id = 9, Name = "Phone Charging", Unit = "hours", EmissionFactor = 0.050m, ActivityTypeId = 3, IsActive = true },
                new Activity { Id = 10, Name = "Laptop Use", Unit = "hours", EmissionFactor = 0.080m, ActivityTypeId = 3, IsActive = true },
                new Activity { Id = 11, Name = "Heating Use", Unit = "hours", EmissionFactor = 0.300m, ActivityTypeId = 3, IsActive = true },
                new Activity { Id = 12, Name = "Washing Machine", Unit = "cycles", EmissionFactor = 0.600m, ActivityTypeId = 3, IsActive = true },

                new Activity { Id = 13, Name = "Streaming TV", Unit = "hours", EmissionFactor = 0.036m, ActivityTypeId = 4, IsActive = true },
                new Activity { Id = 14, Name = "Gaming Console", Unit = "hours", EmissionFactor = 0.120m, ActivityTypeId = 4, IsActive = true },
                new Activity { Id = 15, Name = "Cinema Trip", Unit = "trips", EmissionFactor = 1.800m, ActivityTypeId = 4, IsActive = true },
                new Activity { Id = 16, Name = "Gym Travel", Unit = "miles", EmissionFactor = 0.411m, ActivityTypeId = 4, IsActive = true }
            );

            modelBuilder.Entity<Tips>().HasData(

                // ----------------------------
                // TRANSPORT TIPS
                // ----------------------------
                new Tips
                {
                    Id = 1,
                    Title = "Reduce Car Journeys",
                    Content = "Try walking, cycling, or public transport for shorter journeys to reduce travel emissions.",
                    ActivityTypeId = 1,
                    VideoUrl = null,
                    IsFaq = false
                },
                new Tips
                {
                    Id = 2,
                    Title = "Use Public Transport",
                    Content = "Buses and trains produce significantly less CO2 per passenger than cars.",
                    ActivityTypeId = 1,
                    VideoUrl = null,
                    IsFaq = false
                },
                new Tips
                {
                    Id = 3,
                    Title = "Combine Trips",
                    Content = "Plan routes efficiently to reduce total distance travelled.",
                    ActivityTypeId = 1,
                    VideoUrl = null,
                    IsFaq = false
                },

                // ----------------------------
                // FOOD TIPS
                // ----------------------------
                new Tips
                {
                    Id = 4,
                    Title = "Eat Less Red Meat",
                    Content = "Reducing beef consumption can significantly lower your carbon footprint.",
                    ActivityTypeId = 2,
                    VideoUrl = null,
                    IsFaq = false
                },
                new Tips
                {
                    Id = 5,
                    Title = "Try Meat-Free Days",
                    Content = "Even one meat-free day a week can make a difference.",
                    ActivityTypeId = 2,
                    VideoUrl = null,
                    IsFaq = false
                },
                new Tips
                {
                    Id = 6,
                    Title = "Reduce Food Waste",
                    Content = "Plan meals and store food properly to avoid unnecessary waste.",
                    ActivityTypeId = 2,
                    VideoUrl = null,
                    IsFaq = false
                },

                // ----------------------------
                // ENERGY TIPS
                // ----------------------------
                new Tips
                {
                    Id = 7,
                    Title = "Turn Off Standby Devices",
                    Content = "Electronics still use power in standby mode—switch them off completely.",
                    ActivityTypeId = 3,
                    VideoUrl = null,
                    IsFaq = false
                },
                new Tips
                {
                    Id = 8,
                    Title = "Lower Heating Usage",
                    Content = "Reducing your heating by even 1°C can cut energy consumption.",
                    ActivityTypeId = 3,
                    VideoUrl = null,
                    IsFaq = false
                },
                new Tips
                {
                    Id = 9,
                    Title = "Use Energy Efficient Appliances",
                    Content = "Modern appliances use significantly less energy than older ones.",
                    ActivityTypeId = 3,
                    VideoUrl = null,
                    IsFaq = false
                },

                // ----------------------------
                // LEISURE TIPS
                // ----------------------------
                new Tips
                {
                    Id = 10,
                    Title = "Reduce Streaming Quality",
                    Content = "Lowering video quality reduces data usage and energy consumption.",
                    ActivityTypeId = 4,
                    VideoUrl = null,
                    IsFaq = false
                },
                new Tips
                {
                    Id = 11,
                    Title = "Limit Gaming Time",
                    Content = "Gaming consoles can consume significant power during long sessions.",
                    ActivityTypeId = 4,
                    VideoUrl = null,
                    IsFaq = false
                },
                new Tips
                {
                    Id = 12,
                    Title = "Choose Outdoor Activities",
                    Content = "Outdoor activities have a much lower environmental impact.",
                    ActivityTypeId = 4,
                    VideoUrl = null,
                    IsFaq = false
                },

                // ----------------------------
                // GENERAL TIPS (no activity type)
                // ----------------------------
                new Tips
                {
                    Id = 13,
                    Title = "Start Small",
                    Content = "Small daily changes can add up to a significant reduction over time.",
                    ActivityTypeId = null,
                    VideoUrl = "https://www.youtube.com/embed/BNzgS95r8U8",
                    IsFaq = false
                },
                new Tips
                {
                    Id = 14,
                    Title = "Track Your Progress",
                    Content = "Regularly reviewing your emissions helps you stay motivated.",
                    ActivityTypeId = null,
                    VideoUrl = null,
                    IsFaq = false
                },

                // ============================
                // FAQ SECTION
                // ============================

                new Tips
                {
                    Id = 15,
                    Title = "How is my carbon footprint calculated?",
                    Content = "Your emissions are calculated using predefined emission factors multiplied by your activity quantity.",
                    ActivityTypeId = null,
                    VideoUrl = null,
                    IsFaq = true
                },
                new Tips
                {
                    Id = 16,
                    Title = "Can I edit my entries?",
                    Content = "Yes. You can edit or delete entries from the Emissions Log page.",
                    ActivityTypeId = null,
                    VideoUrl = null,
                    IsFaq = true
                },
                new Tips
                {
                    Id = 17,
                    Title = "Why do I see recommended tips?",
                    Content = "Recommendations are based on the activity type contributing the most CO2 in your logs.",
                    ActivityTypeId = null,
                    VideoUrl = null,
                    IsFaq = true
                },
                new Tips
                {
                    Id = 18,
                    Title = "Do I need to log every day?",
                    Content = "No, but regular logging improves the accuracy of insights and recommendations.",
                    ActivityTypeId = null,
                    VideoUrl = null,
                    IsFaq = true
                },
                new Tips
                {
                    Id = 19,
                    Title = "Is my data secure?",
                    Content = "Your account uses Auth0 authentication and your data is stored securely.",
                    ActivityTypeId = null,
                    VideoUrl = null,
                    IsFaq = true
                }
            );
        }
    }
}