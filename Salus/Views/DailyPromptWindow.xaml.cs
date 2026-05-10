using Salus.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Salus.Views
{
    public partial class DailyPromptWindow : Window
    {
        private readonly DailyPromptViewModel _vm;
        private readonly Button[] _moodButtons;
        private bool _isResolving;

        public DailyPromptWindow(DailyPromptViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            DataContext = vm;

            _moodButtons = new[] { Mood1, Mood2, Mood3, Mood4, Mood5 };

            vm.CloseRequested += () => { _isResolving = true; Close(); };
            vm.OpenCameraRequested += OpenCamera;

            Closing += async (_, e) =>
            {
                if (!_isResolving)
                {
                    _isResolving = true;
                    await _vm.DismissCommand.ExecuteAsync(null);
                }
            };
        }

        protected override async void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            await _vm.LoadAsync();
        }

        private void MoodButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string tagStr && int.TryParse(tagStr, out int rating))
            {
                _vm.SetMood(rating);
                for (int i = 0; i < _moodButtons.Length; i++)
                {
                    _moodButtons[i].Style = (i + 1) <= rating
                        ? (Style)FindResource("MoodButtonActive")
                        : (Style)FindResource("MoodButton");
                }
            }
        }

        private void SkipPhoto_Click(object sender, RoutedEventArgs e)
        {
            _vm.SetPhoto(string.Empty);
        }

        private async System.Threading.Tasks.Task OpenCamera()
        {
            var cameraWin = App.ServiceProvider.GetService(typeof(CameraPreviewWindow)) as CameraPreviewWindow;
            if (cameraWin == null) return;

            if (cameraWin.ShowDialog() == true && cameraWin.CapturedPath != null)
            {
                _vm.SetPhoto(cameraWin.CapturedPath);
                var bmp = new BitmapImage(new Uri(cameraWin.CapturedPath));
                PhotoPreview.Source = bmp;
            }

            await System.Threading.Tasks.Task.CompletedTask;
        }
    }
}
