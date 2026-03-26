using System;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace Berichthefte_WPF.Helpers
{
    public enum Theme
    {
        Light,
        Dark
    }

    public static class ThemeManager
    {
        private static Theme _currentTheme = Theme.Light;
        public static event EventHandler? ThemeChanged;
        private static readonly string _themeFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "theme_preference.json");

        public static Theme CurrentTheme
        {
            get => _currentTheme;
            set
            {
                if (_currentTheme != value)
                {
                    _currentTheme = value;
                    ApplyTheme(value);
                    SaveThemePreference(value);
                    ThemeChanged?.Invoke(null, EventArgs.Empty);
                }
            }
        }

        public static void Initialize()
        {
            var savedTheme = LoadThemePreference();
            _currentTheme = savedTheme;
            ApplyTheme(savedTheme);
        }

        private static void ApplyTheme(Theme theme)
        {
            var resourceDictionary = Application.Current.Resources;

            if (theme == Theme.Dark)
            {
                // Dark Mode Colors
                resourceDictionary["PrimaryBrush"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(100, 181, 246)); // Light Blue
                resourceDictionary["AccentBrush"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(33, 33, 33)); // Very Dark Gray
                resourceDictionary["BorderBrushCustom"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(66, 66, 66)); // Dark Gray
                resourceDictionary["CardBackgroundBrush"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(48, 48, 48)); // Dark Gray
                resourceDictionary["ButtonBrush"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(100, 181, 246)); // Light Blue
                resourceDictionary["ButtonHoverBrush"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(66, 165, 245)); // Medium Blue
                resourceDictionary["WindowBackground"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(33, 33, 33)); // Very Dark Gray
                resourceDictionary["TextForeground"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(229, 229, 229)); // Light Gray
                resourceDictionary["TextBoxBackground"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(66, 66, 66)); // Dark Gray
                resourceDictionary["TextBoxForeground"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(229, 229, 229)); // Light Gray
            }
            else
            {
                // Light Mode Colors
                resourceDictionary["PrimaryBrush"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(47, 93, 159)); // Dark Blue
                resourceDictionary["AccentBrush"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(234, 242, 255)); // Light Blue
                resourceDictionary["BorderBrushCustom"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(214, 220, 229)); // Light Gray
                resourceDictionary["CardBackgroundBrush"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255)); // White
                resourceDictionary["ButtonBrush"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(47, 93, 159)); // Dark Blue
                resourceDictionary["ButtonHoverBrush"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(36, 73, 126)); // Darker Blue
                resourceDictionary["WindowBackground"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(245, 247, 251)); // Very Light Blue
                resourceDictionary["TextForeground"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(31, 41, 55)); // Dark Gray
                resourceDictionary["TextBoxBackground"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255)); // White
                resourceDictionary["TextBoxForeground"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(31, 41, 55)); // Dark Gray
            }
        }

        private static void SaveThemePreference(Theme theme)
        {
            try
            {
                var data = new { Theme = theme.ToString() };
                var json = JsonSerializer.Serialize(data);
                File.WriteAllText(_themeFile, json);
            }
            catch { }
        }

        private static Theme LoadThemePreference()
        {
            try
            {
                if (File.Exists(_themeFile))
                {
                    var json = File.ReadAllText(_themeFile);
                    using (JsonDocument doc = JsonDocument.Parse(json))
                    {
                        var themeStr = doc.RootElement.GetProperty("Theme").GetString();
                        if (themeStr != null && Enum.TryParse<Theme>(themeStr, out var theme))
                        {
                            return theme;
                        }
                    }
                }
            }
            catch { }
            
            return Theme.Light;
        }
    }
}
