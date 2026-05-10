using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Salus.Models;
using Salus.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Salus.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly ProfileService _profileService;
        private readonly ExerciseService _exerciseService;
        private readonly ThemeService _themeService;
        private readonly SessionContext _session;

        [ObservableProperty] private ObservableCollection<Profile> _profiles = new();
        [ObservableProperty] private ObservableCollection<Exercise> _exercises = new();
        [ObservableProperty] private string _newProfileName = string.Empty;
        [ObservableProperty] private string _newExerciseName = string.Empty;
        [ObservableProperty] private string _selectedTheme = "Light";
        [ObservableProperty] private bool _launchOnStartup;

        private const string RegistryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string AppName = "Salus";

        public SettingsViewModel(ProfileService profileService, ExerciseService exerciseService,
            ThemeService themeService, SessionContext session)
        {
            _profileService = profileService;
            _exerciseService = exerciseService;
            _themeService = themeService;
            _session = session;
        }

        public async Task LoadAsync()
        {
            var profiles = await _profileService.GetAllProfilesAsync();
            Profiles = new ObservableCollection<Profile>(profiles);

            if (_session.ActiveProfile != null)
            {
                var exercises = await _exerciseService.GetExercisesAsync(_session.ActiveProfile.Id);
                Exercises = new ObservableCollection<Exercise>(exercises);
                SelectedTheme = _session.ActiveProfile.ThemePreference ?? "Light";
            }

            using var key = Registry.CurrentUser.OpenSubKey(RegistryKey, false);
            LaunchOnStartup = key?.GetValue(AppName) != null;
        }

        [RelayCommand]
        private async Task CreateProfile()
        {
            if (string.IsNullOrWhiteSpace(NewProfileName)) return;
            var profile = await _profileService.CreateProfileAsync(NewProfileName.Trim());
            Profiles.Insert(0, profile);
            NewProfileName = string.Empty;
        }

        [RelayCommand]
        private async Task DeleteProfile(Profile? profile)
        {
            if (profile == null) return;
            await _profileService.DeleteProfileAsync(profile.Id);
            Profiles.Remove(profile);
        }

        [RelayCommand]
        private async Task SelectProfile(Profile? profile)
        {
            if (profile == null) return;
            await _profileService.SetActiveProfileAsync(profile.Id);
            _session.ActiveProfile = profile;
            var exercises = await _exerciseService.GetExercisesAsync(profile.Id);
            Exercises = new ObservableCollection<Exercise>(exercises);
        }

        [RelayCommand]
        private async Task AddExercise()
        {
            if (string.IsNullOrWhiteSpace(NewExerciseName) || _session.ActiveProfile == null) return;
            var ex = await _exerciseService.AddExerciseAsync(_session.ActiveProfile.Id, NewExerciseName.Trim());
            Exercises.Add(ex);
            NewExerciseName = string.Empty;
        }

        [RelayCommand]
        private async Task DeleteExercise(Exercise? exercise)
        {
            if (exercise == null) return;
            await _exerciseService.DeleteExerciseAsync(exercise.Id);
            Exercises.Remove(exercise);
        }

        [RelayCommand]
        private async Task ApplyTheme()
        {
            _themeService.ApplyTheme(SelectedTheme);
            if (_session.ActiveProfile != null)
                await _profileService.SaveThemeAsync(_session.ActiveProfile.Id, SelectedTheme, string.Empty);
        }

        [RelayCommand]
        private void ToggleStartup()
        {
            using var key = Registry.CurrentUser.OpenSubKey(RegistryKey, true);
            if (key == null) return;

            if (LaunchOnStartup)
            {
                var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty;
                key.SetValue(AppName, $"\"{exePath}\"");
            }
            else
            {
                key.DeleteValue(AppName, false);
            }
        }
    }
}
