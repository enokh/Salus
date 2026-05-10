namespace Salus.Models
{
    public class ExerciseLog
    {
        public int Id { get; set; }
        public int DailyEntryId { get; set; }
        public int ExerciseId { get; set; }

        public DailyEntry DailyEntry { get; set; } = null!;
        public Exercise Exercise { get; set; } = null!;
    }
}
