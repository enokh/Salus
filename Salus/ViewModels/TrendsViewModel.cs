using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Salus.Services;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Salus.ViewModels
{
    public partial class TrendsViewModel : ObservableObject
    {
        private readonly EntryService _entryService;
        private readonly SessionContext _session;

        [ObservableProperty] private DateTime _startDate = DateTime.Today.AddDays(-30);
        [ObservableProperty] private DateTime _endDate = DateTime.Today;
        [ObservableProperty] private ISeries[] _series = Array.Empty<ISeries>();
        [ObservableProperty] private Axis[] _xAxes = Array.Empty<Axis>();

        [ObservableProperty] private bool _showSleep = true;
        [ObservableProperty] private bool _showExercise = true;
        [ObservableProperty] private bool _showWeight = true;
        [ObservableProperty] private bool _showMood = true;

        public TrendsViewModel(EntryService entryService, SessionContext session)
        {
            _entryService = entryService;
            _session = session;
        }

        partial void OnStartDateChanged(DateTime value) => _ = LoadDataAsync();
        partial void OnEndDateChanged(DateTime value) => _ = LoadDataAsync();
        partial void OnShowSleepChanged(bool value) => _ = LoadDataAsync();
        partial void OnShowExerciseChanged(bool value) => _ = LoadDataAsync();
        partial void OnShowWeightChanged(bool value) => _ = LoadDataAsync();
        partial void OnShowMoodChanged(bool value) => _ = LoadDataAsync();

        [RelayCommand]
        public async Task LoadDataAsync()
        {
            if (_session.ActiveProfile == null) return;

            var entries = await _entryService.GetEntriesForRangeAsync(
                _session.ActiveProfile.Id, StartDate, EndDate);

            var allDates = Enumerable.Range(0, (EndDate - StartDate).Days + 1)
                .Select(i => StartDate.AddDays(i))
                .ToList();

            var entryMap = entries.ToDictionary(e => e.Date.Date);

            var seriesList = new List<ISeries>();

            if (ShowSleep)
            {
                seriesList.Add(new LineSeries<DateTimePoint>
                {
                    Name = "Sleep (hrs)",
                    Values = allDates.Select(d => new DateTimePoint(d,
                        entryMap.TryGetValue(d, out var e) && e.SleepHours.HasValue ? e.SleepHours : null)).ToArray(),
                    Stroke = new SolidColorPaint(SKColors.SteelBlue, 2),
                    Fill = null,
                    GeometrySize = 6
                });
            }

            if (ShowExercise)
            {
                seriesList.Add(new LineSeries<DateTimePoint>
                {
                    Name = "Exercise (min)",
                    Values = allDates.Select(d => new DateTimePoint(d,
                        entryMap.TryGetValue(d, out var e) && e.ExerciseDuration.HasValue ? (double?)e.ExerciseDuration : null)).ToArray(),
                    Stroke = new SolidColorPaint(SKColors.SeaGreen, 2),
                    Fill = null,
                    GeometrySize = 6
                });
            }

            if (ShowWeight)
            {
                seriesList.Add(new LineSeries<DateTimePoint>
                {
                    Name = "Weight (kg)",
                    Values = allDates.Select(d => new DateTimePoint(d,
                        entryMap.TryGetValue(d, out var e) && e.WeightKg.HasValue ? e.WeightKg : null)).ToArray(),
                    Stroke = new SolidColorPaint(SKColors.Tomato, 2),
                    Fill = null,
                    GeometrySize = 6
                });
            }

            if (ShowMood)
            {
                seriesList.Add(new LineSeries<DateTimePoint>
                {
                    Name = "Mood (1–5)",
                    Values = allDates.Select(d => new DateTimePoint(d,
                        entryMap.TryGetValue(d, out var e) && e.MoodRating.HasValue ? (double?)e.MoodRating : null)).ToArray(),
                    Stroke = new SolidColorPaint(SKColors.MediumOrchid, 2),
                    Fill = null,
                    GeometrySize = 6
                });
            }

            Series = seriesList.ToArray();
            XAxes = new[]
            {
                new DateTimeAxis(TimeSpan.FromDays(1), date => date.ToString("MMM d"))
            };
        }
    }
}
