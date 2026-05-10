using Microsoft.EntityFrameworkCore;
using Salus.Models;
using System;
using System.IO;

namespace Salus.Data
{
    public class SalusDbContext : DbContext
    {
        public DbSet<Profile> Profiles => Set<Profile>();
        public DbSet<DailyEntry> DailyEntries => Set<DailyEntry>();
        public DbSet<Exercise> Exercises => Set<Exercise>();
        public DbSet<ExerciseLog> ExerciseLogs => Set<ExerciseLog>();

        public SalusDbContext(DbContextOptions<SalusDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Profile>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Name).IsRequired().HasMaxLength(100);
                e.HasMany(p => p.DailyEntries).WithOne(d => d.Profile).HasForeignKey(d => d.ProfileId).OnDelete(DeleteBehavior.Cascade);
                e.HasMany(p => p.Exercises).WithOne(ex => ex.Profile).HasForeignKey(ex => ex.ProfileId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<DailyEntry>(e =>
            {
                e.HasKey(d => d.Id);
                e.HasIndex(d => new { d.ProfileId, d.Date }).IsUnique();
                e.HasMany(d => d.ExerciseLogs).WithOne(l => l.DailyEntry).HasForeignKey(l => l.DailyEntryId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Exercise>(e =>
            {
                e.HasKey(ex => ex.Id);
                e.Property(ex => ex.Name).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<ExerciseLog>(e =>
            {
                e.HasKey(l => l.Id);
                e.HasOne(l => l.Exercise).WithMany().HasForeignKey(l => l.ExerciseId).OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
