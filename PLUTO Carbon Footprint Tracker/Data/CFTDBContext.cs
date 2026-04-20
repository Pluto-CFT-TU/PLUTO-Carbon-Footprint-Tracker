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
                new ActivityType { Id = 3, Name = "Energy" }
);

            modelBuilder.Entity<Activity>().HasData(
                new Activity { Id = 1, Name = "Car Travel", Unit = "miles", EmissionFactor = 0.411m, ActivityTypeId = 1 },
                new Activity { Id = 2, Name = "Bus Travel", Unit = "miles", EmissionFactor = 0.105m, ActivityTypeId = 1 },
                new Activity { Id = 3, Name = "Beef Meal", Unit = "meals", EmissionFactor = 2.5m, ActivityTypeId = 2 }
            );
            modelBuilder.Entity<Tips>().HasData(
    new Tips
    {
        Id = 1,
        Title = "Reduce Car Journeys",
        Content = "Try walking, cycling, or public transport for shorter journeys to reduce travel emissions.",
        ActivityTypeId = 1,
        VideoUrl = null
    },
    new Tips
    {
        Id = 2,
        Title = "Save Home Energy",
        Content = "Turn off appliances when not in use and reduce heating where possible to cut energy consumption.",
        ActivityTypeId = 3,
        VideoUrl = null
    },
    new Tips
    {
        Id = 3,
        Title = "Start Small",
        Content = "Making small changes to transport, food, and energy habits can reduce your carbon footprint over time.",
        ActivityTypeId = null,
        VideoUrl = null
    }
);
        }
    }
}