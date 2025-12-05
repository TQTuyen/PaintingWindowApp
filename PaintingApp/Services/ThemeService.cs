using Microsoft.UI.Xaml;
using PaintingApp.Contracts;

namespace PaintingApp.Services;

public class ThemeService : IThemeService
{
    public ElementTheme CurrentTheme { get; private set; } = ElementTheme.Default;

    public void SetTheme(string themeName)
    {
        var theme = GetElementTheme(themeName);
        CurrentTheme = theme;

        if (App.MainWindow?.Content is FrameworkElement rootElement)
        {
            rootElement.RequestedTheme = theme;
        }
    }

    public void ApplySystemTheme()
    {
        SetTheme("System");
    }

    public string GetThemeName()
    {
        return CurrentTheme switch
        {
            ElementTheme.Light => "Light",
            ElementTheme.Dark => "Dark",
            _ => "System"
        };
    }

    public ElementTheme GetElementTheme(string themeName)
    {
        if (string.IsNullOrWhiteSpace(themeName))
        {
            return ElementTheme.Default;
        }

        return themeName.ToLowerInvariant() switch
        {
            "light" => ElementTheme.Light,
            "dark" => ElementTheme.Dark,
            "system" => ElementTheme.Default,
            _ => ElementTheme.Default
        };
    }
}
