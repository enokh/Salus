using System;
using System.Collections.Generic;

namespace Salus.Models
{
    public class DailyEntry
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public DateTime Date { get; set; }
        public double? SleepHours { get; set; }
        public int? ExerciseDuration { get; set; }
        public double? WeightKg { get; set; }
        public int? MoodRating { get; set; }
        public string? PhotoPath { get; set; }
        public bool IsSkipped { get; set; }

        public Profile Profile { get; set; } = null!;
        public ICollection<ExerciseLog> ExerciseLogs { get; set; } = new List<ExerciseLog>();
    }
}
