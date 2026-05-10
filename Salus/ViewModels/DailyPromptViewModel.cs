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
    public partial class DailyPromptViewModel : ObservableObject
    {
        private readonly EntryService _entryService;
        private readonly ExerciseService _exerciseService;
        private readonly CameraService _cameraService;
        private readonly SessionContext _session;

        [ObservableProperty] private double? _sleepHours;
        [ObservableProperty] private int? _exerciseDuration;
        [ObservableProperty] private double? _weightKg;
        [ObservableProperty] private int _moodRating;
        [ObservableProperty] private string? _photoPath;
        [ObservableProperty] private DailyEntry? _previousEntry;
        [ObservableProperty] private ObservableCollection<ExerciseSelectionItem> _exercises = new();
        [ObservableProperty] private bool _cameraAvailable;
        [ObservableProperty] private bool _hasPhoto;

        public event Action? CloseRequested;
        public event Func<Task>? OpenCameraRequested;

        public DailyPromptViewModel(EntryService entryService, ExerciseService exerciseService,
            CameraService cameraService, SessionContext session)
        {
            _entryService = entryService;
            _exerciseService = exerciseService;
            _cameraService = cameraService;
            _session = session;
        }

        public async Task LoadAsync()
        {
            if (_session.ActiveProfile == null) return;

            PreviousEntry = await _entryService.GetMostRecentEntryAsync(_session.ActiveProfile.Id);
            CameraAvailable = _cameraService.IsCameraAvailable();

            var exerciseList = await _exerciseService.GetExercisesAsync(_session.ActiveProfile.Id);
            Exercises = new ObservableCollection<ExerciseSelectionItem>(
                exerciseList.Select(e => new ExerciseSelectionItem(e)));
        }

        [RelayCommand]
        private async Task Submit()
        {
            if (_session.ActiveProfile == null) return;

            var selectedExercises = Exercises.Where(e => e.IsSelected).Select(e => e.Exercise).ToList();
            var photoPath = PhotoPath;

            if (string.IsNullOrEmpty(photoPath))
                photoPath = _cameraService.CopyPlaceholderPhoto(_session.ActiveProfile.Id, DateTime.Today);

            var entry = new DailyEntry
            {
                ProfileId = _session.ActiveProfile.Id,
                Date = DateTime.Today,
                SleepHours = SleepHours,
                ExerciseDuration = ExerciseDuration,
                WeightKg = WeightKg,
                MoodRating = MoodRating == 0 ? null : MoodRating,
                PhotoPath = photoPath,
                IsSkipped = false,
                ExerciseLogs = selectedExercises.Select(e => new ExerciseLog { ExerciseId = e.Id }).ToList()
            };

            await _entryService.SaveEntryAsync(entry);
            _session.HasShownPromptToday = true;
            CloseRequested?.Invoke();
        }

        [RelayCommand]
        private async Task Dismiss()
        {
            if (_session.ActiveProfile == null) return;
            await _entryService.MarkDaySkippedAsync(_session.ActiveProfile.Id, DateTime.Today);
            _session.HasShownPromptToday = true;
            CloseRequested?.Invoke();
        }

        [RelayCommand]
        private async Task TakePhoto()
        {
            if (OpenCameraRequested != null)
                await OpenCameraRequested();
        }

        public void SetMood(int rating) => MoodRating = rating;

        public void SetPhoto(string path)
        {
            PhotoPath = path;
            HasPhoto = !string.IsNullOrEmpty(path);
        }
    }

    public partial class ExerciseSelectionItem : ObservableObject
    {
        public Exercise Exercise { get; }
        [ObservableProperty] private bool _isSelected;

        public ExerciseSelectionItem(Exercise exercise)
        {
            Exercise = exercise;
        }
    }
}
