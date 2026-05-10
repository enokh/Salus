using AForge.Video;
using AForge.Video.DirectShow;
using Salus.Services;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Salus.Views
{
    public partial class CameraPreviewWindow : Window
    {
        private VideoCaptureDevice? _device;
        private Bitmap? _lastFrame;
        private readonly CameraService _cameraService;
        private readonly SessionContext _session;

        public string? CapturedPath { get; private set; }

        public CameraPreviewWindow(CameraService cameraService, SessionContext session)
        {
            InitializeComponent();
            _cameraService = cameraService;
            _session = session;
            Loaded += OnLoaded;
            Closing += OnClosing;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (devices.Count == 0) { Close(); return; }

                _device = new VideoCaptureDevice(devices[0].MonikerString);
                _device.NewFrame += OnNewFrame;
                _device.Start();
            }
            catch
            {
                Close();
            }
        }

        private void OnNewFrame(object sender, NewFrameEventArgs e)
        {
            _lastFrame?.Dispose();
            _lastFrame = (Bitmap)e.Frame.Clone();

            Dispatcher.Invoke(() =>
            {
                using var ms = new MemoryStream();
                _lastFrame.Save(ms, ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.StreamSource = ms;
                bmp.EndInit();
                bmp.Freeze();
                CameraFeed.Source = bmp;
            });
        }

        private void Capture_Click(object sender, RoutedEventArgs e)
        {
            if (_lastFrame == null || _session.ActiveProfile == null) return;

            StopCamera();
            var path = _cameraService.GetPhotoStoragePath(_session.ActiveProfile.Id, DateTime.Today);
            _lastFrame.Save(path, ImageFormat.Jpeg);
            CapturedPath = path;
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            StopCamera();
            DialogResult = false;
        }

        private void OnClosing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            StopCamera();
        }

        private void StopCamera()
        {
            if (_device != null && _device.IsRunning)
            {
                _device.NewFrame -= OnNewFrame;
                _device.SignalToStop();
                _device.WaitForStop();
            }
        }
    }
}
