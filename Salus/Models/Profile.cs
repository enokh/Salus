using System;
using System.Collections.Generic;

namespace Salus.Models
{
    public class Profile
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime LastUsedAt { get; set; }
        public string? ThemePreference { get; set; }
        public string? AccentColor { get; set; }

        public ICollection<DailyEntry> DailyEntries { get; set; } = new List<DailyEntry>();
        public ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
    }
}
