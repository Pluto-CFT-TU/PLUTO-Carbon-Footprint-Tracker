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
        }
    }
}

