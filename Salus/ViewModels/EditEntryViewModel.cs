using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Salus.Models;
using Salus.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Salus.ViewModels
{
    public partial class EditEntryViewModel : ObservableObject
    {
        private readonly EntryService _entryService;
        private readonly ExerciseService _exerciseService;
        private readonly SessionContext _session;

        [ObservableProperty] private DateTime _entryDate;
        [ObservableProperty] private double? _sleepHours;
        [ObservableProperty] private int? _exerciseDuration;
        [ObservableProperty] private double? _weightKg;
        [ObservableProperty] private int _moodRating;
        [ObservableProperty] private string? _photoPath;
        [ObservableProperty] private ObservableCollection<ExerciseSelectionItem> _exercises = new();

        public event Action? SaveCompleted;

        private int? _existingEntryId;

        public EditEntryViewModel(EntryService entryService, ExerciseService exerciseService, SessionContext session)
        {
            _entryService = entryService;
            _exerciseService = exerciseService;
            _session = session;
        }

        public async Task LoadAsync(DailyEntry? entry)
        {
            if (_session.ActiveProfile == null) return;

            EntryDate = entry?.Date ?? DateTime.Today;
            _existingEntryId = entry?.Id;

            SleepHours = entry?.SleepHours;
            ExerciseDuration = entry?.ExerciseDuration;
            WeightKg = entry?.WeightKg;
            MoodRating = entry?.MoodRating ?? 0;
            PhotoPath = entry?.PhotoPath;

            var exerciseList = await _exerciseService.GetExercisesAsync(_session.ActiveProfile.Id);
            var selectedIds = entry?.ExerciseLogs.Select(l => l.ExerciseId).ToHashSet() ?? new();
            Exercises = new ObservableCollection<ExerciseSelectionItem>(
                exerciseList.Select(e => new ExerciseSelectionItem(e) { IsSelected = selectedIds.Contains(e.Id) }));
        }

        [RelayCommand]
        private async Task SaveChanges()
        {
            if (_session.ActiveProfile == null) return;

            var selectedExercises = Exercises.Where(e => e.IsSelected).Select(e => e.Exercise).ToList();
            var entry = new DailyEntry
            {
                ProfileId = _session.ActiveProfile.Id,
                Date = EntryDate,
                SleepHours = SleepHours,
                ExerciseDuration = ExerciseDuration,
                WeightKg = WeightKg,
                MoodRating = MoodRating == 0 ? null : MoodRating,
                PhotoPath = PhotoPath,
                IsSkipped = false,
                ExerciseLogs = selectedExercises.Select(e => new ExerciseLog { ExerciseId = e.Id }).ToList()
            };

            await _entryService.SaveEntryAsync(entry);
            SaveCompleted?.Invoke();
        }

        public void SetMood(int rating) => MoodRating = rating;
    }
}
