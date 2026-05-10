using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace Salus.Services
{
    public class PromptSchedulerService : IDisposable
    {
        private readonly EntryService _entryService;
        private readonly ProfileService _profileService;
        private readonly SessionContext _session;
        private Timer? _timer;
        private bool _firedToday;
        private DateTime _lastCheckDate;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT { public int Left, Top, Right, Bottom; }

        public event Action? PromptRequested;

        public PromptSchedulerService(EntryService entryService, ProfileService profileService, SessionContext session)
        {
            _entryService = entryService;
            _profileService = profileService;
            _session = session;
        }

        public void Start()
        {
            _timer = new Timer(CheckAsync, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
        }

        private async void CheckAsync(object? state)
        {
            var today = DateTime.Today;
            if (_lastCheckDate != today)
            {
                _firedToday = false;
                _lastCheckDate = today;
            }

            if (_firedToday) return;
            if (DateTime.Now.Hour < 5) return;
            if (_session.ActiveProfile == null) return;

            var hasEntry = await _entryService.HasEntryForTodayAsync(_session.ActiveProfile.Id);
            if (hasEntry) { _firedToday = true; return; }

            _firedToday = true;

            Application.Current.Dispatcher.Invoke(() => PromptRequested?.Invoke());
        }

        public void Dispose() => _timer?.Dispose();
    }
}
