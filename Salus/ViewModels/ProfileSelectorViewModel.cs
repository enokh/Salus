using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Salus.Models;
using Salus.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Salus.ViewModels
{
    public partial class ProfileSelectorViewModel : ObservableObject
    {
        private readonly ProfileService _profileService;
        private readonly SessionContext _session;

        [ObservableProperty] private ObservableCollection<Profile> _profiles = new();
        [ObservableProperty] private Profile? _selectedProfile;
        [ObservableProperty] private string _newProfileName = string.Empty;
        [ObservableProperty] private bool _isCreating;

        public event System.Action? ProfileSelected;

        public ProfileSelectorViewModel(ProfileService profileService, SessionContext session)
        {
            _profileService = profileService;
            _session = session;
        }

        public async Task LoadAsync()
        {
            var profiles = await _profileService.GetAllProfilesAsync();
            Profiles = new ObservableCollection<Profile>(profiles);
        }

        [RelayCommand]
        private async Task SelectProfile(Profile? profile)
        {
            if (profile == null) return;
            await _profileService.SetActiveProfileAsync(profile.Id);
            _session.ActiveProfile = profile;
            ProfileSelected?.Invoke();
        }

        [RelayCommand]
        private async Task CreateProfile()
        {
            if (string.IsNullOrWhiteSpace(NewProfileName)) return;
            var profile = await _profileService.CreateProfileAsync(NewProfileName.Trim());
            Profiles.Insert(0, profile);
            NewProfileName = string.Empty;
            IsCreating = false;
            await SelectProfile(profile);
        }

        [RelayCommand]
        private async Task DeleteProfile(Profile? profile)
        {
            if (profile == null) return;
            await _profileService.DeleteProfileAsync(profile.Id);
            Profiles.Remove(profile);
            if (_session.ActiveProfile?.Id == profile.Id)
                _session.ActiveProfile = null;
        }

        [RelayCommand]
        private void ShowCreateForm() => IsCreating = true;

        [RelayCommand]
        private void CancelCreate()
        {
            IsCreating = false;
            NewProfileName = string.Empty;
        }
    }
}
