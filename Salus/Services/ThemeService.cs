using System;
using System.Windows;

namespace Salus.Services
{
    public class ThemeService
    {
        private const string LightThemeUri = "Resources/LightTheme.xaml";
        private const string DarkThemeUri = "Resources/DarkTheme.xaml";

        public string CurrentTheme { get; private set; } = "Light";

        public void ApplyTheme(string theme)
        {
            CurrentTheme = theme;
            var uri = theme == "Dark"
                ? new Uri(DarkThemeUri, UriKind.Relative)
                : new Uri(LightThemeUri, UriKind.Relative);

            var dict = new ResourceDictionary { Source = uri };

            var existing = FindThemeDictionary();
            if (existing != null)
                Application.Current.Resources.MergedDictionaries.Remove(existing);

            Application.Current.Resources.MergedDictionaries.Add(dict);
        }

        private ResourceDictionary? FindThemeDictionary()
        {
            foreach (var dict in Application.Current.Resources.MergedDictionaries)
            {
                if (dict.Source != null &&
                    (dict.Source.OriginalString.Contains("LightTheme") ||
                     dict.Source.OriginalString.Contains("DarkTheme")))
                    return dict;
            }
            return null;
        }
    }
}
