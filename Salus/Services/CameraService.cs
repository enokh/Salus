using AForge.Video.DirectShow;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace Salus.Services
{
    public class CameraService
    {
        public bool IsCameraAvailable()
        {
            try
            {
                var devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                return devices.Count > 0;
            }
            catch
            {
                return false;
            }
        }

        public string GetPhotoStoragePath(int profileId, DateTime date)
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Salus", "Photos", profileId.ToString());
            Directory.CreateDirectory(dir);
            return Path.Combine(dir, $"{date:yyyy-MM-dd}.jpg");
        }

        public string CopyPlaceholderPhoto(int profileId, DateTime date)
        {
            var destPath = GetPhotoStoragePath(profileId, date);
            if (!File.Exists(destPath))
            {
                var uri = new Uri("pack://application:,,,/Resources/placeholder.png");
                var sri = Application.GetResourceStream(uri);
                if (sri != null)
                {
                    using var stream = sri.Stream;
                    using var img = Image.FromStream(stream);
                    img.Save(destPath, ImageFormat.Jpeg);
                }
            }
            return destPath;
        }
    }
}
