using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Salus.Models;
using Salus.Services;
using System;
using System.Threading.Tasks;

namespace Salus.ViewModels
{
    public partial class HistoryViewModel : ObservableObject
    {
        private readonly EntryService _entryService;
        private readonly SessionContext _session;

        [ObservableProperty] private DateTime _selectedDate = DateTime.Today;
        [ObservableProperty] private DailyEntry? _currentEntry;
        [ObservableProperty] private bool _hasEntry;
        [ObservableProperty] private bool _isSkipped;
        [ObservableProperty] private bool _isEmpty;

        public event Action<DailyEntry?>? EditRequested;

        public HistoryViewModel(EntryService entryService, SessionContext session)
        {
            _entryService = entryService;
            _session = session;
        }

        partial void OnSelectedDateChanged(DateTime value) => _ = LoadEntryAsync();

        public async Task LoadEntryAsync()
        {
            if (_session.ActiveProfile == null) return;

            CurrentEntry = await _entryService.GetEntryForDateAsync(_session.ActiveProfile.Id, SelectedDate);
            HasEntry = CurrentEntry != null;
            IsSkipped = CurrentEntry?.IsSkipped == true;
            IsEmpty = CurrentEntry == null;
        }

        [RelayCommand]
        private void Edit()
        {
            EditRequested?.Invoke(CurrentEntry);
        }
    }
}
