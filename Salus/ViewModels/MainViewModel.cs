using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace Salus.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty] private object? _currentView;

        private readonly HistoryViewModel _historyVM;
        private readonly TrendsViewModel _trendsVM;
        private readonly SettingsViewModel _settingsVM;

        public MainViewModel(HistoryViewModel historyVM, TrendsViewModel trendsVM, SettingsViewModel settingsVM)
        {
            _historyVM = historyVM;
            _trendsVM = trendsVM;
            _settingsVM = settingsVM;
        }

        [RelayCommand]
        public async Task ShowHistory()
        {
            CurrentView = _historyVM;
            await _historyVM.LoadEntryAsync();
        }

        [RelayCommand]
        public async Task ShowTrends()
        {
            CurrentView = _trendsVM;
            await _trendsVM.LoadDataAsync();
        }

        [RelayCommand]
        public async Task ShowSettings()
        {
            CurrentView = _settingsVM;
            await _settingsVM.LoadAsync();
        }
    }
}
