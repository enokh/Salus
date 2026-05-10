using Microsoft.EntityFrameworkCore;
using Salus.Data;
using Salus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salus.Services
{
    public class EntryService
    {
        private readonly SalusDbContext _db;

        public EntryService(SalusDbContext db)
        {
            _db = db;
        }

        public async Task<DailyEntry?> GetEntryForDateAsync(int profileId, DateTime date)
        {
            var day = date.Date;
            return await _db.DailyEntries
                .Include(e => e.ExerciseLogs).ThenInclude(l => l.Exercise)
                .FirstOrDefaultAsync(e => e.ProfileId == profileId && e.Date == day);
        }

        public async Task<DailyEntry?> GetMostRecentEntryAsync(int profileId)
            => await _db.DailyEntries
                .Where(e => e.ProfileId == profileId && !e.IsSkipped)
                .OrderByDescending(e => e.Date)
                .FirstOrDefaultAsync();

        public async Task<DailyEntry> SaveEntryAsync(DailyEntry entry)
        {
            entry.Date = entry.Date.Date;
            var existing = await _db.DailyEntries
                .Include(e => e.ExerciseLogs)
                .FirstOrDefaultAsync(e => e.ProfileId == entry.ProfileId && e.Date == entry.Date);

            if (existing == null)
            {
                _db.DailyEntries.Add(entry);
            }
            else
            {
                existing.SleepHours = entry.SleepHours;
                existing.ExerciseDuration = entry.ExerciseDuration;
                existing.WeightKg = entry.WeightKg;
                existing.MoodRating = entry.MoodRating;
                existing.PhotoPath = entry.PhotoPath;
                existing.IsSkipped = entry.IsSkipped;

                _db.ExerciseLogs.RemoveRange(existing.ExerciseLogs);
                foreach (var log in entry.ExerciseLogs)
                {
                    log.DailyEntryId = existing.Id;
                    _db.ExerciseLogs.Add(log);
                }
            }

            await _db.SaveChangesAsync();
            return existing ?? entry;
        }

        public async Task MarkDaySkippedAsync(int profileId, DateTime date)
        {
            var day = date.Date;
            var existing = await _db.DailyEntries
                .FirstOrDefaultAsync(e => e.ProfileId == profileId && e.Date == day);

            if (existing == null)
            {
                _db.DailyEntries.Add(new DailyEntry
                {
                    ProfileId = profileId,
                    Date = day,
                    IsSkipped = true
                });
            }
            else
            {
                existing.IsSkipped = true;
            }

            await _db.SaveChangesAsync();
        }

        public async Task<bool> HasEntryForTodayAsync(int profileId)
        {
            var today = DateTime.Today;
            return await _db.DailyEntries.AnyAsync(e => e.ProfileId == profileId && e.Date == today);
        }

        public async Task<List<DailyEntry>> GetEntriesForRangeAsync(int profileId, DateTime start, DateTime end)
            => await _db.DailyEntries
                .Include(e => e.ExerciseLogs).ThenInclude(l => l.Exercise)
                .Where(e => e.ProfileId == profileId && e.Date >= start.Date && e.Date <= end.Date)
                .OrderBy(e => e.Date)
                .ToListAsync();
    }
}
