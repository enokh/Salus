using Salus.Models;
using Salus.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Salus.Views
{
    public partial class EditEntryWindow : Window
    {
        private readonly EditEntryViewModel _vm;
        private readonly Button[] _moodButtons;

        public EditEntryWindow(EditEntryViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            DataContext = vm;
            _moodButtons = new[] { EMood1, EMood2, EMood3, EMood4, EMood5 };
            vm.SaveCompleted += () => { DialogResult = true; Close(); };
        }

        public async void LoadEntry(DailyEntry? entry)
        {
            await _vm.LoadAsync(entry);
            UpdateMoodButtons(_vm.MoodRating);
        }

        private void MoodButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string tagStr && int.TryParse(tagStr, out int rating))
            {
                _vm.SetMood(rating);
                UpdateMoodButtons(rating);
            }
        }

        private void UpdateMoodButtons(int rating)
        {
            for (int i = 0; i < _moodButtons.Length; i++)
            {
                _moodButtons[i].Style = (i + 1) <= rating
                    ? (Style)FindResource("MoodButtonActive")
                    : (Style)FindResource("MoodButton");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();
    }
}
