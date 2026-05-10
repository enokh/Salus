using Microsoft.EntityFrameworkCore;
using Salus.Data;
using Salus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salus.Services
{
    public class ProfileService
    {
        private readonly SalusDbContext _db;

        public ProfileService(SalusDbContext db)
        {
            _db = db;
        }

        public async Task<List<Profile>> GetAllProfilesAsync()
            => await _db.Profiles.OrderByDescending(p => p.LastUsedAt).ToListAsync();

        public async Task<Profile> CreateProfileAsync(string name)
        {
            var profile = new Profile
            {
                Name = name,
                CreatedAt = DateTime.UtcNow,
                LastUsedAt = DateTime.UtcNow
            };
            _db.Profiles.Add(profile);
            await _db.SaveChangesAsync();
            return profile;
        }

        public async Task DeleteProfileAsync(int id)
        {
            var profile = await _db.Profiles.FindAsync(id);
            if (profile != null)
            {
                _db.Profiles.Remove(profile);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<Profile?> GetLastActiveProfileAsync()
            => await _db.Profiles.OrderByDescending(p => p.LastUsedAt).FirstOrDefaultAsync();

        public async Task SetActiveProfileAsync(int id)
        {
            var profile = await _db.Profiles.FindAsync(id);
            if (profile != null)
            {
                profile.LastUsedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }
        }

        public async Task RenameProfileAsync(int id, string newName)
        {
            var profile = await _db.Profiles.FindAsync(id);
            if (profile != null)
            {
                profile.Name = newName;
                await _db.SaveChangesAsync();
            }
        }

        public async Task SaveThemeAsync(int profileId, string theme, string accentColor)
        {
            var profile = await _db.Profiles.FindAsync(profileId);
            if (profile != null)
            {
                profile.ThemePreference = theme;
                profile.AccentColor = accentColor;
                await _db.SaveChangesAsync();
            }
        }
    }
}
